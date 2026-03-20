using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using RentAPlace.Data;
using RentAPlace.Models;
using RentAPlace.Models.DTOs;
using System.Security.Claims;

namespace RentAPlace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PropertyController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProperties()
        {
            var list = await _context.Properties
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .ToListAsync();
            return Ok(list.Select(ToDto).ToList());
        }

        [HttpGet("top-rated")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopRatedProperties([FromQuery] string? propertyType, [FromQuery] int count = 10)
        {
            var q = _context.Properties.AsNoTracking().Include(p => p.Owner).Include(p => p.Images).AsQueryable();
            if (!string.IsNullOrWhiteSpace(propertyType))
                q = q.Where(p => p.PropertyType == propertyType);
            var results = await q.OrderByDescending(p => p.Rating).Take(count).ToListAsync();
            return Ok(results.Select(ToDto).ToList());
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchProperties(
            [FromQuery] DateTime? checkIn,
            [FromQuery] DateTime? checkOut,
            [FromQuery] string? location,
            [FromQuery] string? propertyType,
            [FromQuery] List<int>? featureIds)
        {
            var q = _context.Properties
                .AsNoTracking()
                .Include(p => p.Images)
                .Include(p => p.PropertyFeatures!)
                .ThenInclude(pf => pf.Feature)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(location))
            {
                var loc = location.Trim().ToLower();
                q = q.Where(p => p.Location != null && p.Location.ToLower().Contains(loc));
            }
            if (!string.IsNullOrWhiteSpace(propertyType))
            {
                var pt = propertyType.Trim().ToLower();
                q = q.Where(p => p.PropertyType != null && p.PropertyType.ToLower() == pt);
            }
            if (featureIds != null && featureIds.Count > 0)
                q = q.Where(p => p.PropertyFeatures!.Any(pf => featureIds.Contains(pf.FeatureId)));

            if (checkIn.HasValue && checkOut.HasValue)
            {
                var from = checkIn.Value.Date;
                var to = checkOut.Value.Date;
                q = q.Where(p =>
                    !_context.Reservations.Any(r =>
                        r.PropertyId == p.Id &&
                        r.Status != "Rejected" &&
                        r.CheckInDate < to &&
                        r.CheckOutDate > from));
            }

            var list = await q.Include(p => p.Owner).ToListAsync();
            return Ok(list.Select(ToDto).ToList());
        }

        [HttpGet("my")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetMyProperties()
        {
            var owner = await GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var mine = await _context.Properties
                .AsNoTracking()
                .Where(p => p.OwnerId == owner.Id)
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .ToListAsync();
            return Ok(mine.Select(ToDto).ToList());
        }

        [HttpGet("{id:int}/booked-dates")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyBookedDates(int id)
        {
            var dates = await _context.Reservations
                .AsNoTracking()
                .Where(r => r.PropertyId == id && r.Status != "Rejected" && r.CheckInDate.HasValue && r.CheckOutDate.HasValue)
                .Select(r => new { r.CheckInDate, r.CheckOutDate })
                .ToListAsync();
            var ranges = dates
                .Where(x => x.CheckInDate != null && x.CheckOutDate != null)
                .Select(x => new { checkIn = x.CheckInDate!.Value.ToString("yyyy-MM-dd"), checkOut = x.CheckOutDate!.Value.ToString("yyyy-MM-dd") })
                .ToList();
            return Ok(ranges);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProperty(int id)
        {
            var prop = await _context.Properties
                .AsNoTracking()
                .Include(p => p.Images)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (prop == null)
                return NotFound();
            return Ok(ToDto(prop));
        }

        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> CreateProperty(Property property)
        {
            var owner = await GetCurrentOwner();
            if (owner == null) return Unauthorized();

            property.OwnerId = owner.Id;
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            var created = await _context.Properties
                .AsNoTracking()
                .Include(p => p.Owner)
                .Include(p => p.Images)
                .FirstAsync(p => p.Id == property.Id);
            return Ok(ToDto(created));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UpdateProperty(int id, Property property)
        {
            if (id != property.Id) return BadRequest();
            var owner = await GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var existing = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == owner.Id);
            if (existing == null)
                return NotFound("Property not found or you are not the owner.");

            existing.Title = property.Title;
            existing.Description = property.Description;
            existing.Location = property.Location;
            existing.PropertyType = property.PropertyType;
            existing.PricePerNight = property.PricePerNight;
            existing.Rating = property.Rating;
            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var owner = await GetCurrentOwner();
            if (owner == null) return Unauthorized();

            var prop = await _context.Properties.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == owner.Id);
            if (prop == null)
                return NotFound("Property not found or you are not the owner.");
            _context.Properties.Remove(prop);
            await _context.SaveChangesAsync();
            return Ok("Property deleted");
        }

        [HttpPost("{id}/images")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> UploadImages(int id, [FromForm] List<IFormFile> images)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email)) return Unauthorized();

            var prop = await _context.Properties
                .Include(p => p.Owner)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id && p.Owner != null && p.Owner.Email == email);
            if (prop == null) return NotFound("Property not found or you are not the owner.");
            if (images == null || !images.Any()) return BadRequest("No images provided.");

            var uploadDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "images", "properties");
            if (!Directory.Exists(uploadDir))
                Directory.CreateDirectory(uploadDir);

            var added = new List<PropertyImage>();
            foreach (var img in images)
            {
                if (img.Length <= 0) continue;
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var path = Path.Combine(uploadDir, fileName);
                await using (var stream = new FileStream(path, FileMode.Create))
                    await img.CopyToAsync(stream);

                var record = new PropertyImage { PropertyId = prop.Id, ImageUrl = $"/images/properties/{fileName}" };
                _context.PropertyImages.Add(record);
                added.Add(record);
            }
            await _context.SaveChangesAsync();
            return Ok(added);
        }

        private async Task<User?> GetCurrentOwner()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        private static PropertyResponseDto ToDto(Property p)
        {
            return new PropertyResponseDto
            {
                Id = p.Id,
                Title = p.Title ?? "",
                Description = p.Description ?? "",
                Location = p.Location ?? "",
                PropertyType = p.PropertyType ?? "",
                PricePerNight = p.PricePerNight,
                Rating = p.Rating,
                OwnerId = p.OwnerId,
                Owner = p.Owner == null ? null : new OwnerSummaryDto
                {
                    Id = p.Owner.Id,
                    Name = p.Owner.Name ?? "",
                    Email = p.Owner.Email ?? ""
                },
                Images = (p.Images ?? new List<PropertyImage>())
                    .Select(i => new PropertyImageDto { Id = i.Id, PropertyId = i.PropertyId, ImageUrl = i.ImageUrl ?? "" })
                    .ToList()
            };
        }
    }
}

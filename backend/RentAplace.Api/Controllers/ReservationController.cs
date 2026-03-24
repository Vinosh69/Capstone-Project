using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RentAPlace.Data;
using RentAPlace.Models;
using RentAPlace.Models.DTOs;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;

namespace RentAPlace.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public ReservationController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetReservations()
        {
            var list = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Property)
                .Include(r => r.Renter)
                .ToListAsync();
            return Ok(list.Select(r => ToDto(r)).ToList());
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyReservations()
        {
            var renter = await CurrentUser();
            if (renter == null) return Unauthorized();

            var list = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Property)
                .Where(r => r.RenterId == renter.Id)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();
            // Renter view doesn't need renter info repeated in each row.
            return Ok(list.Select(r => ToDto(r, includeRenter: false)).ToList());
        }

        [HttpGet("owner")]
        public async Task<IActionResult> GetOwnerReservations()
        {
            var owner = await CurrentUser();
            if (owner == null) return Unauthorized();

            var list = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Property)
                .Include(r => r.Renter)
                .Where(r => r.Property != null && r.Property.OwnerId == owner.Id)
                .OrderByDescending(r => r.CheckInDate)
                .ToListAsync();
            return Ok(list.Select(r => ToDto(r)).ToList());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var r = await _context.Reservations
                .AsNoTracking()
                .Include(x => x.Property)
                .Include(x => x.Renter)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound("Reservation not found");
            return Ok(ToDto(r));
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(Reservation reservation)
        {
            if (reservation == null) return BadRequest("Invalid reservation");
            if (!reservation.CheckInDate.HasValue || !reservation.CheckOutDate.HasValue)
                return BadRequest("Check-in and check-out dates are required.");
            if (reservation.CheckInDate.Value.Date >= reservation.CheckOutDate.Value.Date)
                return BadRequest("Check-out date must be after check-in date.");

            var renter = await CurrentUser();
            if (renter == null) return Unauthorized();

            reservation.RenterId = renter.Id;
            reservation.Status = "Pending";

            var property = await _context.Properties.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == reservation.PropertyId);
            if (property == null || property.Owner == null)
                return BadRequest("Property not found or has no owner.");

            var from = reservation.CheckInDate.Value.Date;
            var to = reservation.CheckOutDate.Value.Date;
            var overlap = await _context.Reservations.AnyAsync(r =>
                r.PropertyId == reservation.PropertyId &&
                r.Status != "Rejected" &&
                r.CheckInDate < to &&
                r.CheckOutDate > from);
            if (overlap)
                return BadRequest("Property is not available for the selected dates.");

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            await NotifyOwnerNewReservation(property, renter, reservation);
            reservation.Property = property;
            reservation.Renter = renter;
            return Ok(ToDto(reservation));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, Reservation reservation)
        {
            if (id != reservation.Id) return BadRequest("Reservation ID mismatch");
            var current = await CurrentUser();
            if (current == null) return Unauthorized();

            var existing = await _context.Reservations
                .Include(r => r.Property)
                .ThenInclude(p => p!.Owner)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (existing == null) return NotFound("Reservation not found");
            if (existing.Property?.OwnerId != current.Id)
                return Forbid("You are not allowed to modify this reservation.");

            existing.Status = reservation.Status;
            existing.CheckInDate = reservation.CheckInDate;
            existing.CheckOutDate = reservation.CheckOutDate;
            await _context.SaveChangesAsync();

            if (reservation.Status == "Confirmed" || reservation.Status == "Rejected")
            {
                var withRenter = await _context.Reservations.Include(r => r.Property).Include(r => r.Renter).FirstOrDefaultAsync(r => r.Id == id);
                if (withRenter?.Renter != null)
                    await NotifyRenterOfStatus(withRenter);
            }
            existing.Property = existing.Property;
            return Ok(ToDto(existing));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var current = await CurrentUser();
            if (current == null) return Unauthorized();

            var r = await _context.Reservations.Include(x => x.Property).FirstOrDefaultAsync(x => x.Id == id);
            if (r == null) return NotFound("Reservation not found");
            var canCancel = r.RenterId == current.Id || (r.Property != null && r.Property.OwnerId == current.Id);
            if (!canCancel)
                return Forbid("You are not allowed to cancel this reservation.");

            _context.Reservations.Remove(r);
            await _context.SaveChangesAsync();
            return Ok("Reservation removed");
        }

        private async Task<User?> CurrentUser()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        private static ReservationResponseDto ToDto(Reservation r, bool includeRenter = true)
        {
            return new ReservationResponseDto
            {
                Id = r.Id,
                PropertyId = r.PropertyId,
                RenterId = r.RenterId,
                Status = r.Status ?? "Pending",
                CheckInDate = r.CheckInDate,
                CheckOutDate = r.CheckOutDate,
                Property = r.Property == null ? null : new ReservationPropertyDto
                {
                    Id = r.Property.Id,
                    Title = r.Property.Title ?? "",
                    Location = r.Property.Location ?? "",
                    PropertyType = r.Property.PropertyType ?? "",
                    PricePerNight = r.Property.PricePerNight,
                    OwnerId = r.Property.OwnerId
                },
                Renter = !includeRenter || r.Renter == null ? null : new ReservationRenterDto
                {
                    Id = r.Renter.Id,
                    Name = r.Renter.Name ?? "",
                    Email = r.Renter.Email ?? ""
                }
            };
        }

        private async Task NotifyOwnerNewReservation(Property property, User renter, Reservation res)
        {
            var toEmail = property.Owner?.Email;
            if (string.IsNullOrWhiteSpace(toEmail)) return;
            if (!TryGetSmtpSettings(out var host, out var port, out var user, out var pass, out var fromAddr))
                return;

            using var client = new SmtpClient(host, port) { EnableSsl = true, Credentials = new NetworkCredential(user, pass) };
            var body =
                $"Hello {property.Owner?.Name},\n\nA new reservation has been requested for \"{property.Title}\".\n\n" +
                $"Renter: {renter.Name} ({renter.Email})\nCheck-in: {res.CheckInDate:yyyy-MM-dd}\nCheck-out: {res.CheckOutDate:yyyy-MM-dd}\nStatus: {res.Status}\n\n" +
                "Log in to the app to confirm or reject.\n\nRegards,\nRentAPlace";
            var msg = new MailMessage(fromAddr, toEmail, $"New reservation request for {property.Title}", body);
            await client.SendMailAsync(msg);
        }

        private async Task NotifyRenterOfStatus(Reservation res)
        {
            if (res.Renter == null || string.IsNullOrWhiteSpace(res.Renter.Email)) return;
            if (!TryGetSmtpSettings(out var host, out var port, out var user, out var pass, out var fromAddr))
                return;

            using var client = new SmtpClient(host, port) { EnableSsl = true, Credentials = new NetworkCredential(user, pass) };
            var title = res.Property?.Title ?? "your booked property";
            var subject = res.Status == "Confirmed" ? $"Reservation confirmed – {title}" : $"Reservation not confirmed – {title}";
            var body = res.Status == "Confirmed"
                ? $"Hello {res.Renter.Name},\n\nYour reservation for \"{title}\" has been confirmed.\n\nCheck-in: {res.CheckInDate:yyyy-MM-dd}\nCheck-out: {res.CheckOutDate:yyyy-MM-dd}\n\nRegards,\nRentAPlace"
                : $"Hello {res.Renter.Name},\n\nYour reservation for \"{title}\" was not confirmed (rejected).\n\nRegards,\nRentAPlace";
            var msg = new MailMessage(fromAddr, res.Renter.Email, subject, body);
            await client.SendMailAsync(msg);
        }

        private bool TryGetSmtpSettings(out string host, out int port, out string user, out string pass, out string from)
        {
            host = _config["Smtp:Host"] ?? "";
            var portStr = _config["Smtp:Port"];
            user = _config["Smtp:User"] ?? "";
            pass = _config["Smtp:Pass"] ?? "";
            from = _config["Smtp:From"] ?? "";
            port = 587;
            if (int.TryParse(portStr, out var p)) port = p;
            return !string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(pass) && !string.IsNullOrWhiteSpace(from);
        }
    }
}

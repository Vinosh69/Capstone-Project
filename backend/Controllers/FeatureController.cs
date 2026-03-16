using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentAPlace.Data;
using RentAPlace.Models;

namespace RentAPlace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FeatureController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all features
        [HttpGet]
        public async Task<IActionResult> GetFeatures()
        {
            var features = await _context.Features.AsNoTracking().ToListAsync();

            return Ok(features);
        }

        // Add feature
        [HttpPost]
        public async Task<IActionResult> AddFeature(Feature feature)
        {
            _context.Features.Add(feature);

            await _context.SaveChangesAsync();

            return Ok(feature);
        }

        // Delete feature
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeature(int id)
        {
            var feature = await _context.Features.FindAsync(id);

            if (feature == null)
                return NotFound();

            _context.Features.Remove(feature);

            await _context.SaveChangesAsync();

            return Ok("Feature removed");
        }
    }
}
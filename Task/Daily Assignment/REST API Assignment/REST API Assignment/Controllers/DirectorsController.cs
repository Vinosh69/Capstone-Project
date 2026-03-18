using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST_API_Assignment.Data;
using REST_API_Assignment.Models;

namespace REST_API_Assignment.Controllers
{
    [Route("api/directors")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DirectorsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Director>>> GetDirectors()
        {
            return await _context.Directors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Director>> GetDirector(int id)
        {
            var director = await _context.Directors.FindAsync(id);

            if (director == null)
                return NotFound();

            return director;
        }

        [HttpPost]
        public async Task<ActionResult<Director>> CreateDirector(Director director)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Directors.Add(director);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDirector), new { id = director.Id }, director);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDirector(int id, Director director)
        {
            if (id != director.Id)
                return BadRequest();

            _context.Entry(director).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDirector(int id)
        {
            var director = await _context.Directors.FindAsync(id);

            if (director == null)
                return NotFound();

            _context.Directors.Remove(director);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/movies")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMoviesByDirector(int id)
        {
            var exists = await _context.Directors.AnyAsync(d => d.Id == id);

            if (!exists)
                return NotFound();

            return await _context.Movies
                .Where(m => m.DirectorId == id)
                .ToListAsync();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using RentAPlace.Data;
using RentAPlace.Models;
using System.Security.Claims;

namespace RentAPlace.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public MessageController(ApplicationDbContext context)
        {
            _db = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            var me = await GetMe();
            if (me == null) return Unauthorized();

            var list = await _db.Messages
                .AsNoTracking()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Property)
                .Where(m => m.SenderId == me.Id || m.ReceiverId == me.Id)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(Message message)
        {
            if (message == null) return BadRequest("Invalid message");
            var me = await GetMe();
            if (me == null) return Unauthorized();

            message.SenderId = me.Id;
            message.CreatedAt = DateTime.UtcNow;
            _db.Messages.Add(message);
            await _db.SaveChangesAsync();
            return Ok(message);
        }

        private async Task<User?> GetMe()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}

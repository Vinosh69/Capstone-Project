using mywebapi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace YourProject.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class PartiesController : ControllerBase
        {
            // Static list for demo (replace with database later)
            private static List<Party> parties = new List<Party>
        {
            new Party { Id = 1, Name = "Internal Party A", IsExternal = false },
            new Party { Id = 2, Name = "Internal Party B", IsExternal = false },
            new Party { Id = 3, Name = "External Party X", IsExternal = true }
        };

            // GET: api/parties
            [HttpGet]
            public ActionResult<IEnumerable<Party>> GetParties()
            {
                return Ok(parties);
            }

            // GET: api/parties/5
            [HttpGet("{id}")]
            public ActionResult<Party> GetParty(int id)
            {
                var party = parties.FirstOrDefault(p => p.Id == id);

                if (party == null)
                    return NotFound();

                return Ok(party);
            }

            // POST: api/parties/signup
            [HttpPost("signup")]
            public IActionResult SignUp([FromBody] int partyId)
            {
                var party = parties.FirstOrDefault(p => p.Id == partyId);

                if (party == null)
                    return NotFound("Party not found");

                if (party.IsExternal)
                    return BadRequest("External party cannot sign up");

                return Ok("Signup successful");
            }
        }
    }



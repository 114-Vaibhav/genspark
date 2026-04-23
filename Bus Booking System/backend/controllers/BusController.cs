using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("bus")]
    public class BusController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BusController(AppDbContext context)
        {
            _context = context;
        }

        // Operator adds bus

        [Authorize(Roles = "Operator")]
        [HttpPost("/bus")]
        public async Task<IActionResult> AddBus(Bus bus)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var operatorId = Guid.Parse(userIdClaim);

            var op = await _context.Operators
                .FirstOrDefaultAsync(o => o.UserId == operatorId && o.Approved);

            if (op == null)
                return BadRequest("Operator not approved");

            bus.Id = Guid.NewGuid();
            bus.OperatorId = op.Id;

            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();

            return Ok(bus);
        }

        // Public view buses
        [HttpGet]
        public async Task<IActionResult> GetBuses()
        {
            return Ok(await _context.Buses.ToListAsync());
        }
    }
}

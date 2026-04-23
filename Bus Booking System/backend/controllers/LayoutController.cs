using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("layout")]
    public class LayoutController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LayoutController(AppDbContext context)
        {
            _context = context;
        }

        // Create layout
        [Authorize(Roles = "Admin,Operator")]
        [HttpPost]
        public async Task<IActionResult> CreateLayout([FromBody] SeatLayout layout)
        {
            layout.Id = Guid.NewGuid();
            _context.SeatLayouts.Add(layout);
            await _context.SaveChangesAsync();

            return Ok(layout);
        }

        // Add seats to layout
        [Authorize(Roles = "Admin,Operator")]
        [HttpPost("seat")]
        public async Task<IActionResult> AddSeat([FromBody] Seat seat)
        {
            seat.Id = Guid.NewGuid();
            _context.Seats.Add(seat);
            await _context.SaveChangesAsync();

            return Ok(seat);
        }

        // Get layout with seats
        [HttpGet("{layoutId}")]
        public async Task<IActionResult> GetLayout(Guid layoutId)
        {
            var seats = await _context.Seats
                .Where(s => s.LayoutId == layoutId)
                .ToListAsync();

            return Ok(seats);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("trip")]
    public class TripController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TripController(AppDbContext context)
        {
            _context = context;
        }

        // Create trip (Operator/Admin)
        [Authorize(Roles = "Operator,Admin")]
        [HttpPost("/trip")]
        public async Task<IActionResult> CreateTrip(Trip trip)
        {
            var bus = await _context.Buses.FindAsync(trip.BusId);
            if (bus == null)
                return BadRequest("Invalid bus");

            trip.Id = Guid.NewGuid();

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            return Ok(trip);
        }

        // Search trips
        [HttpGet("/trip-search")]
        public async Task<IActionResult> Search(string source, string destination, DateTime date)
        {
            var routes = await _context.Routes
                .Where(r => r.Source.ToLower().Contains(source.ToLower()) &&
                            r.Destination.ToLower().Contains(destination.ToLower()))
                .ToListAsync();

            var routeIds = routes.Select(r => r.Id);

            var trips = await _context.Trips
                .Where(t => routeIds.Contains(t.RouteId) &&
                            t.JourneyDate.Date == date.Date)
                .ToListAsync();

            return Ok(trips);
        }
    }
}
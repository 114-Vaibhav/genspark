using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RouteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/routes")]
        public async Task<IActionResult> GetRoutes()
        {
            var routes = await _context.Routes.ToListAsync();
            return Ok(routes);
        }

        [HttpGet("/route-search")]
        public async Task<IActionResult> Search(string source, string destination)
        {
            var result = await _context.Routes
                .Where(r => r.Source.ToLower().Contains(source.ToLower())
                         && r.Destination.ToLower().Contains(destination.ToLower()))
                .ToListAsync();

            return Ok(result);
        }
    }
}
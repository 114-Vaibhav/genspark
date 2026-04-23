using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("/secure")]
        public IActionResult Secure()
        {
            return Ok("You are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/admin")]
        public IActionResult Admin()
        {
            return Ok("Admin only access");
        }
    }
}
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
            Console.WriteLine("[Test/Secure] /secure endpoint called");
            return Ok("You are authenticated");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/admin")]
        public IActionResult Admin()
        {
            Console.WriteLine("[Test/Admin] /admin endpoint called");
            return Ok("Admin only access");
        }
    }
}

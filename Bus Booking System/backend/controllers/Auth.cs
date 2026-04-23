using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    public class AuthRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
    }

    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _secret = "THIS_IS_MY_SECRET_KEY_12345";

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("/auth/register")]
        public async Task<IActionResult> Register([FromBody] AuthRequest request)
        {
            var exists = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (exists != null)
                return BadRequest("User already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                MobileNo = request.MobileNo,
                Gender = request.Gender,
                DOB = request.DOB,
                Role = request.Role ?? "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // If Operator → create Operator entry
            if (user.Role == "Operator")
            {
                _context.Operators.Add(new Operator
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Approved = false,
                    IsActive = false
                });

                await _context.SaveChangesAsync();
            }

            return Ok("Registered successfully");
        }

        [HttpPost("/auth/login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(ClaimTypes.Name, user.Email),
                    new System.Security.Claims.Claim(ClaimTypes.Role, user.Role),
                    new System.Security.Claims.Claim("UserId", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                role = user.Role
            });
        }

        [HttpGet("/test")]
        public IActionResult Test()
        {
            return Ok("Backend working 🚀");
        }
    }
}

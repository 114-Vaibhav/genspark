using Microsoft.AspNetCore.Mvc;
using backend.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using backend.DTOs;
using backend.Models;
using Npgsql;
using backend.Security;

namespace backend.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;
        private readonly string _secret = JwtSettings.SecretKey;

        public AuthController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            Console.WriteLine($"[Auth/Register] Request received for email={request.Email}, role={request.Role}");
            var normalizedRole = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role.Trim();
            if (normalizedRole != "User" && normalizedRole != "Operator")
                return BadRequest("Only User or Operator registration is allowed");

            var existingUserId = await _sql.ExecuteScalarAsync(
                "SELECT \"Id\" FROM \"Users\" WHERE \"Email\" = @email LIMIT 1;",
                new NpgsqlParameter("email", request.Email));

            if (existingUserId != null)
            {
                Console.WriteLine($"[Auth/Register] User already exists for email={request.Email}");
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                MobileNo = request.MobileNo,
                Gender = request.Gender,
                DOB = request.Dob,
                Role = normalizedRole,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            await _sql.WithTransactionAsync(async (connection, transaction) =>
            {
                await using var insertUser = _sql.CreateCommand(
                    connection,
                    @"INSERT INTO ""Users"" (""Id"", ""Name"", ""Email"", ""MobileNo"", ""Gender"", ""DOB"", ""Role"", ""PasswordHash"")
                      VALUES (@id, @name, @email, @mobileNo, @gender, @dob, @role, @passwordHash);",
                    transaction,
                    new NpgsqlParameter("id", user.Id),
                    new NpgsqlParameter("name", user.Name),
                    new NpgsqlParameter("email", user.Email),
                    new NpgsqlParameter("mobileNo", user.MobileNo),
                    new NpgsqlParameter("gender", user.Gender),
                    new NpgsqlParameter("dob", user.DOB),
                    new NpgsqlParameter("role", user.Role),
                    new NpgsqlParameter("passwordHash", user.PasswordHash));

                await insertUser.ExecuteNonQueryAsync();

                if (user.Role == "Operator")
                {
                    await using var insertOperator = _sql.CreateCommand(
                        connection,
                        @"INSERT INTO ""Operators"" (""Id"", ""UserId"", ""Approved"", ""IsActive"", ""Address"")
                          VALUES (@id, @userId, @approved, @isActive, @address);",
                        transaction,
                        new NpgsqlParameter("id", Guid.NewGuid()),
                        new NpgsqlParameter("userId", user.Id),
                        new NpgsqlParameter("approved", false),
                        new NpgsqlParameter("isActive", false),
                        new NpgsqlParameter("address", string.Empty));

                    await insertOperator.ExecuteNonQueryAsync();
                }

                return 0;
            });

            Console.WriteLine($"[Auth/Register] Registration successful for email={request.Email}, userId={user.Id}");
            return Ok("Registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"[Auth/Login] Login request received for email={request.Email}");
            var user = await _sql.QuerySingleOrDefaultAsync(
                @"SELECT ""Id"", ""Name"", ""Email"", ""Role"", ""PasswordHash""
                  FROM ""Users""
                  WHERE ""Email"" = @email
                  LIMIT 1;",
                reader => new User
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Role = reader.GetString(reader.GetOrdinal("Role")),
                    PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"))
                },
                new NpgsqlParameter("email", request.Email));

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                Console.WriteLine($"[Auth/Login] Invalid credentials for email={request.Email}");
                return Unauthorized("Invalid credentials");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            Console.WriteLine($"[Auth/Login] Login successful for email={request.Email}, userId={user.Id}, role={user.Role}");
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                role = user.Role
            });
        }

        [HttpGet("/test")]
        public IActionResult Test()
        {
            Console.WriteLine("[Auth/Test] /test endpoint called");
            return Ok("Backend working");
        }
    }
}

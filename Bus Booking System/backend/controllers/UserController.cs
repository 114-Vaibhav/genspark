using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using Npgsql;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public UserController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            Console.WriteLine("[User/GetProfile] Fetching profile");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            var user = await _sql.QuerySingleOrDefaultAsync(
                @"SELECT ""Id"", ""Name"", ""Email"", ""MobileNo"", ""Gender"", ""DOB"", ""Role""
                  FROM ""Users""
                  WHERE ""Id"" = @userId
                  LIMIT 1;",
                reader => new
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    MobileNo = reader.GetString(reader.GetOrdinal("MobileNo")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    DOB = reader.GetDateTime(reader.GetOrdinal("DOB")).ToString("yyyy-MM-dd"),
                    Role = reader.GetString(reader.GetOrdinal("Role"))
                },
                new NpgsqlParameter("userId", userId));

            if (user == null) return NotFound("User not found");

            return Ok(user);
        }

        public class UpdateProfileRequest
        {
            public string Name { get; set; } = string.Empty;
            public string MobileNo { get; set; } = string.Empty;
            public string Gender { get; set; } = string.Empty;
            public DateTime DOB { get; set; }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            Console.WriteLine("[User/UpdateProfile] Updating profile");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            await _sql.ExecuteAsync(
                @"UPDATE ""Users""
                  SET ""Name"" = @name,
                      ""MobileNo"" = @mobileNo,
                      ""Gender"" = @gender,
                      ""DOB"" = @dob
                  WHERE ""Id"" = @userId;",
                new NpgsqlParameter("name", request.Name),
                new NpgsqlParameter("mobileNo", request.MobileNo),
                new NpgsqlParameter("gender", request.Gender),
                new NpgsqlParameter("dob", request.DOB),
                new NpgsqlParameter("userId", userId));

            return Ok("Profile updated successfully");
        }
    }
}

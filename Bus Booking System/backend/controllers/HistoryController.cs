using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("history")]
    public class HistoryController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public HistoryController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetHistory()
        {
            Console.WriteLine("[History/GetHistory] Fetching booking history");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var bookings = await _sql.QueryAsync(
                @"SELECT ""Id"", ""UserId"", ""TripId"", ""TotalAmount"", ""Status"", ""CreatedAt""
                  FROM ""Bookings""
                  WHERE ""UserId"" = @userId
                  ORDER BY ""CreatedAt"" DESC;",
                reader => new Booking
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                    TripId = reader.GetGuid(reader.GetOrdinal("TripId")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                },
                new NpgsqlParameter("userId", Guid.Parse(userIdClaim)));

            return Ok(bookings);
        }
    }
}

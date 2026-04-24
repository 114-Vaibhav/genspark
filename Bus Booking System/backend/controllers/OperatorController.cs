using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("operator")]
    [Authorize(Roles = "Operator")]
    public class OperatorController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public OperatorController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [HttpGet("bookings")]
        public async Task<IActionResult> GetOperatorBookings()
        {
            Console.WriteLine("[Operator/GetOperatorBookings] Fetching bookings for operator");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            var bookings = await _sql.QueryAsync(
                @"SELECT bk.""Id"", bk.""TotalAmount"", bk.""PlatformFee"", bk.""Status"", bk.""CreatedAt"",
                         t.""JourneyDate"", r.""Source"", r.""Destination"", b.""BusNumber"", u.""Name"" as ""UserName""
                  FROM ""Bookings"" bk
                  INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                  INNER JOIN ""Buses"" b ON b.""Id"" = t.""BusId""
                  INNER JOIN ""Routes"" r ON r.""Id"" = t.""RouteId""
                  INNER JOIN ""Users"" u ON u.""Id"" = bk.""UserId""
                  INNER JOIN ""Operators"" op ON op.""Id"" = b.""OperatorId""
                  WHERE op.""UserId"" = @userId
                  ORDER BY bk.""CreatedAt"" DESC;",
                reader => new
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                    PlatformFee = reader.GetDecimal(reader.GetOrdinal("PlatformFee")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                    JourneyDate = reader.GetDateTime(reader.GetOrdinal("JourneyDate")).ToString("yyyy-MM-dd"),
                    Source = reader.GetString(reader.GetOrdinal("Source")),
                    Destination = reader.GetString(reader.GetOrdinal("Destination")),
                    BusNumber = reader.GetString(reader.GetOrdinal("BusNumber")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                },
                new NpgsqlParameter("userId", userId));

            return Ok(bookings);
        }

        [HttpGet("trips/{tripId:guid}/bookings")]
        public async Task<IActionResult> GetTripPassengers(Guid tripId)
        {
            Console.WriteLine($"[Operator/GetTripPassengers] Fetching passengers for tripId={tripId}");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            var passengers = await _sql.QueryAsync(
                @"SELECT tr.""Id"", tr.""Name"", tr.""Age"", tr.""Gender"", tr.""SeatNumber"", bk.""Status""
                  FROM ""Travelers"" tr
                  INNER JOIN ""Bookings"" bk ON bk.""Id"" = tr.""BookingId""
                  INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                  INNER JOIN ""Buses"" b ON b.""Id"" = t.""BusId""
                  INNER JOIN ""Operators"" op ON op.""Id"" = b.""OperatorId""
                  WHERE t.""Id"" = @tripId AND op.""UserId"" = @userId
                  ORDER BY tr.""SeatNumber"";",
                reader => new
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Age = reader.GetInt32(reader.GetOrdinal("Age")),
                    Gender = reader.GetString(reader.GetOrdinal("Gender")),
                    SeatNumber = reader.GetString(reader.GetOrdinal("SeatNumber")),
                    BookingStatus = reader.GetString(reader.GetOrdinal("Status"))
                },
                new NpgsqlParameter("tripId", tripId),
                new NpgsqlParameter("userId", userId));

            return Ok(passengers);
        }
    }
}

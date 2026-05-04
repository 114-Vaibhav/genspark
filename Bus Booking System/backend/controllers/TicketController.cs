using Microsoft.AspNetCore.Mvc;
using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("ticket")]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public TicketController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [HttpGet("{bookingId}")]
        public async Task<IActionResult> GetTicket(Guid bookingId)
        {
            Console.WriteLine($"[Ticket/GetTicket] Fetching ticket for bookingId={bookingId}");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var ticket = await _sql.QuerySingleOrDefaultAsync(
                @"
                SELECT
                    bk.""Id"" AS ""BookingId"",
                    bk.""Status"",
                    bk.""TotalAmount"" AS ""Amount"",
                    t.""JourneyDate"",
                    t.""PickupAddress"",
                    t.""DropAddress""
                FROM ""Bookings"" bk
                INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                WHERE bk.""Id"" = @bookingId
                  AND bk.""UserId"" = @userId
                  AND bk.""Status"" = 'Confirmed'
                LIMIT 1;",
                reader => new TicketResponse
                {
                    BookingId = reader.GetGuid(reader.GetOrdinal("BookingId")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                    JourneyDate = reader.GetDateTime(reader.GetOrdinal("JourneyDate")),
                    PickupAddress = reader.IsDBNull(reader.GetOrdinal("PickupAddress")) ? "" : reader.GetString(reader.GetOrdinal("PickupAddress")),
                    DropAddress = reader.IsDBNull(reader.GetOrdinal("DropAddress")) ? "" : reader.GetString(reader.GetOrdinal("DropAddress"))
                },
                new NpgsqlParameter("bookingId", bookingId),
                new NpgsqlParameter("userId", Guid.Parse(userIdClaim)));

            if (ticket == null)
                return BadRequest("Invalid or unconfirmed booking");

            var seats = await _sql.QueryAsync(
                @"SELECT ""SeatNumber""
                  FROM ""Travelers""
                  WHERE ""BookingId"" = @bookingId
                  ORDER BY ""SeatNumber"";",
                reader => reader.GetString(reader.GetOrdinal("SeatNumber")),
                new NpgsqlParameter("bookingId", bookingId));

            ticket.Seats = seats;
            return Ok(ticket);
        }
    }
}

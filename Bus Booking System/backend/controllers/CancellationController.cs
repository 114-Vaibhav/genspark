using Microsoft.AspNetCore.Mvc;
using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("cancel")]
    [Authorize]
    public class CancellationController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;
        private readonly IEmailService _emailService;

        public CancellationController(PostgresSqlRunner sql, IEmailService emailService)
        {
            _sql = sql;
            _emailService = emailService;
        }

        [HttpPost("{bookingId}")]
        public async Task<IActionResult> Cancel(Guid bookingId)
        {
            Console.WriteLine($"[Cancellation/Cancel] Cancellation request received bookingId={bookingId}");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            return await _sql.WithTransactionAsync<IActionResult>(async (connection, transaction) =>
            {
                await using var bookingCommand = _sql.CreateCommand(
                    connection,
                    @"
                    SELECT
                        bk.""TotalAmount"",
                        t.""JourneyDate""
                    FROM ""Bookings"" bk
                    INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                    WHERE bk.""Id"" = @bookingId
                      AND bk.""UserId"" = @userId
                      AND bk.""Status"" = 'Confirmed'
                    LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId),
                    new NpgsqlParameter("userId", Guid.Parse(userIdClaim)));

                await using var reader = await bookingCommand.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return BadRequest("Invalid booking");

                var totalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"));
                var journeyDate = reader.GetDateTime(reader.GetOrdinal("JourneyDate"));
                await reader.CloseAsync();

                var hoursLeft = (journeyDate - DateTime.UtcNow).TotalHours;
                var refund = hoursLeft > 48 ? totalAmount * 0.75m
                    : hoursLeft > 24 ? totalAmount * 0.5m
                    : hoursLeft > 6 ? totalAmount * 0.25m
                    : 0m;

                await using var updateBooking = _sql.CreateCommand(
                    connection,
                    @"UPDATE ""Bookings""
                      SET ""Status"" = 'Cancelled'
                      WHERE ""Id"" = @bookingId;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId));

                await updateBooking.ExecuteNonQueryAsync();

                await using var insertCancellation = _sql.CreateCommand(
                    connection,
                    @"INSERT INTO ""Cancellations"" (""Id"", ""BookingId"", ""RefundAmount"", ""CancelledAt"")
                      VALUES (@id, @bookingId, @refundAmount, @cancelledAt);",
                    transaction,
                    new NpgsqlParameter("id", Guid.NewGuid()),
                    new NpgsqlParameter("bookingId", bookingId),
                    new NpgsqlParameter("refundAmount", refund),
                    new NpgsqlParameter("cancelledAt", DateTime.UtcNow));

                await insertCancellation.ExecuteNonQueryAsync();

                var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "user@example.com";
                var nameClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "User";
                await _emailService.SendBookingCancellationAsync(emailClaim, nameClaim, bookingId.ToString(), refund);

                return Ok(new { refund });
            });
        }
    }
}

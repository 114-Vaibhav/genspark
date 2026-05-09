using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.DTOs;
using Npgsql;
using NpgsqlTypes;

namespace backend.Controllers
{
    public partial class BookingController
    {
        [Authorize]
        [HttpPost("lock-seat")]
        public async Task<IActionResult> LockSeats([FromBody] LockSeatRequest request)
        {
            Console.WriteLine($"[Booking/LockSeats] Request received tripId={request.TripId}");

            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            var seatNumbers = request.SeatNumbers.Distinct().ToArray();

            return await _sql.WithTransactionAsync<IActionResult>(async (connection, transaction) =>
            {
                await using var conflictCommand = _sql.CreateCommand(
                    connection,
                    @"
                    SELECT seat_number
                    FROM (
                        SELECT sl.""SeatNumber"" AS seat_number
                        FROM ""SeatLocks"" sl
                        WHERE sl.""TripId"" = @tripId
                        AND sl.""ExpiryTime"" > @nowUtc
                        AND sl.""SeatNumber"" = ANY(@seatNumbers)
                    ) conflicts
                    LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("tripId", request.TripId),
                    new NpgsqlParameter("nowUtc", DateTime.UtcNow),
                    new NpgsqlParameter<string[]>("seatNumbers", seatNumbers)
                    {
                        NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Text
                    });

                var conflict = await conflictCommand.ExecuteScalarAsync();

                if (conflict != null)
                    return BadRequest($"Seat {conflict} is already locked");

                foreach (var seat in seatNumbers)
                {
                    await using var insertLock = _sql.CreateCommand(
                        connection,
                        @"INSERT INTO ""SeatLocks""
                        (""Id"", ""TripId"", ""SeatNumber"", ""LockedByUserId"", ""ExpiryTime"")
                        VALUES
                        (@id, @tripId, @seatNumber, @userId, @expiryTime);",
                        transaction,
                        new NpgsqlParameter("id", Guid.NewGuid()),
                        new NpgsqlParameter("tripId", request.TripId),
                        new NpgsqlParameter("seatNumber", seat),
                        new NpgsqlParameter("userId", userId),
                        new NpgsqlParameter("expiryTime", DateTime.UtcNow.AddMinutes(5)));

                    await insertLock.ExecuteNonQueryAsync();
                }

                return Ok("Seats locked");
            });
        }
    }
}
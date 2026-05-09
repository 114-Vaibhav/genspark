using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Npgsql;

namespace backend.Controllers
{
    public partial class BookingController
    {
        [Authorize]
        [HttpPost("payment")]
        public async Task<IActionResult> Payment([FromQuery] Guid bookingId)
        {
            Console.WriteLine($"[Booking/Payment] Payment started");

            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            return await _sql.WithTransactionAsync<IActionResult>(async (connection, transaction) =>
            {
                await using var bookingCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT ""TotalAmount""
                    FROM ""Bookings""
                    WHERE ""Id""=@bookingId
                    AND ""UserId""=@userId;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId),
                    new NpgsqlParameter("userId", userId));

                var amountObj = await bookingCommand.ExecuteScalarAsync();

                if (amountObj == null)
                    return BadRequest("Booking not found");

                var amount = Convert.ToDecimal(amountObj);

                await using var paymentInsert = _sql.CreateCommand(
                    connection,
                    @"INSERT INTO ""Payments""
                    (""Id"", ""BookingId"", ""Amount"", ""Status"", ""CreatedAt"")
                    VALUES
                    (@id,@bookingId,@amount,@status,@createdAt);",
                    transaction,
                    new NpgsqlParameter("id", Guid.NewGuid()),
                    new NpgsqlParameter("bookingId", bookingId),
                    new NpgsqlParameter("amount", amount),
                    new NpgsqlParameter("status", "Success"),
                    new NpgsqlParameter("createdAt", DateTime.UtcNow));

                await paymentInsert.ExecuteNonQueryAsync();

                await using var updateBooking = _sql.CreateCommand(
                    connection,
                    @"UPDATE ""Bookings""
                    SET ""Status""='Confirmed'
                    WHERE ""Id""=@bookingId;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId));

                await updateBooking.ExecuteNonQueryAsync();

                return Ok("Payment success");
            });
        }
    }
}
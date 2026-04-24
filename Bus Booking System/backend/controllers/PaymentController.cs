using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using Npgsql;

using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;
        private readonly IEmailService _emailService;

        public PaymentController(PostgresSqlRunner sql, IEmailService emailService)
        {
            _sql = sql;
            _emailService = emailService;
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment([FromQuery] Guid bookingId)
        {
            Console.WriteLine($"[Payment/MakePayment] Request received bookingId={bookingId}");
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
                      WHERE ""Id"" = @bookingId AND ""UserId"" = @userId
                      LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId),
                    new NpgsqlParameter("userId", userId));

                var totalAmountObject = await bookingCommand.ExecuteScalarAsync();
                if (totalAmountObject == null)
                    return BadRequest("Invalid booking");

                var totalAmount = Convert.ToDecimal(totalAmountObject);

                await using var insertPayment = _sql.CreateCommand(
                    connection,
                    @"INSERT INTO ""Payments"" (""Id"", ""BookingId"", ""Amount"", ""Status"", ""PaymentMethod"", ""CreatedAt"")
                      VALUES (@id, @bookingId, @amount, @status, @paymentMethod, @createdAt);",
                    transaction,
                    new NpgsqlParameter("id", Guid.NewGuid()),
                    new NpgsqlParameter("bookingId", bookingId),
                    new NpgsqlParameter("amount", totalAmount),
                    new NpgsqlParameter("status", "Success"),
                    new NpgsqlParameter("paymentMethod", "Dummy"),
                    new NpgsqlParameter("createdAt", DateTime.UtcNow));

                await insertPayment.ExecuteNonQueryAsync();

                await using var updateBooking = _sql.CreateCommand(
                    connection,
                    @"UPDATE ""Bookings""
                      SET ""Status"" = 'Confirmed'
                      WHERE ""Id"" = @bookingId;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId));

                await updateBooking.ExecuteNonQueryAsync();

                await using var tripDetailsCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT r.""Source"", r.""Destination"", b.""BusNumber"", t.""PickupAddress"", t.""DropAddress""
                      FROM ""Bookings"" bk
                      INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                      INNER JOIN ""Routes"" r ON r.""Id"" = t.""RouteId""
                      INNER JOIN ""Buses"" b ON b.""Id"" = t.""BusId""
                      WHERE bk.""Id"" = @bookingId LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId));

                string source = "", dest = "", busNumber = "", pickup = "", drop = "";
                await using (var reader = await tripDetailsCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        source = reader.GetString(0);
                        dest = reader.GetString(1);
                        busNumber = reader.GetString(2);
                        pickup = reader.GetString(3);
                        drop = reader.GetString(4);
                    }
                }

                await using var seatsCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT ""SeatNumber"" FROM ""Travelers"" WHERE ""BookingId"" = @bookingId;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId));
                
                var seatsList = new List<string>();
                await using (var reader = await seatsCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        seatsList.Add(reader.GetString(0));
                    }
                }

                var emailClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "user@example.com";
                var nameClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? "User";
                
                var routeStr = $"{source} to {dest} (Bus: {busNumber}). Pickup: {pickup}, Drop: {drop}";
                var seatsStr = string.Join(", ", seatsList);

                await _emailService.SendBookingConfirmationAsync(emailClaim, nameClaim, bookingId.ToString(), routeStr, seatsStr);

                return Ok(new { message = "Payment success", bookingId });
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Models;
using backend.DTOs;
using Npgsql;
using NpgsqlTypes;

namespace backend.Controllers
{
    public partial class BookingController
    {
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            Console.WriteLine($"[Booking/CreateBooking] Creating booking");

            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            var seatNumbers = request.Travelers
                .Select(x => x.SeatNumber)
                .Distinct()
                .ToArray();

            return await _sql.WithTransactionAsync<IActionResult>(async (connection, transaction) =>
            {
                await using var tripCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT ""Price"" FROM ""Trips""
                    WHERE ""Id""=@tripId LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("tripId", request.TripId));

                var priceObj = await tripCommand.ExecuteScalarAsync();

                if (priceObj == null)
                    return BadRequest("Trip not found");

                var price = Convert.ToDecimal(priceObj);

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TripId = request.TripId,
                    TotalAmount = price * request.Travelers.Count,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                await using var bookingInsert = _sql.CreateCommand(
                    connection,
                    @"INSERT INTO ""Bookings""
                    (""Id"", ""UserId"", ""TripId"", ""TotalAmount"", ""Status"", ""CreatedAt"")
                    VALUES
                    (@id,@userId,@tripId,@amount,@status,@createdAt);",
                    transaction,
                    new NpgsqlParameter("id", booking.Id),
                    new NpgsqlParameter("userId", booking.UserId),
                    new NpgsqlParameter("tripId", booking.TripId),
                    new NpgsqlParameter("amount", booking.TotalAmount),
                    new NpgsqlParameter("status", booking.Status),
                    new NpgsqlParameter("createdAt", booking.CreatedAt));

                await bookingInsert.ExecuteNonQueryAsync();

                foreach (var traveler in request.Travelers)
                {
                    await using var travelerInsert = _sql.CreateCommand(
                        connection,
                        @"INSERT INTO ""Travelers""
                        (""Id"", ""BookingId"", ""Name"", ""Age"", ""Gender"", ""SeatNumber"")
                        VALUES
                        (@id,@bookingId,@name,@age,@gender,@seatNumber);",
                        transaction,
                        new NpgsqlParameter("id", Guid.NewGuid()),
                        new NpgsqlParameter("bookingId", booking.Id),
                        new NpgsqlParameter("name", traveler.Name),
                        new NpgsqlParameter("age", traveler.Age),
                        new NpgsqlParameter("gender", traveler.Gender),
                        new NpgsqlParameter("seatNumber", traveler.SeatNumber));

                    await travelerInsert.ExecuteNonQueryAsync();
                }

                return Ok(booking);
            });
        }
    }
}
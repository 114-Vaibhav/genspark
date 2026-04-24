using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using backend.DTOs;
using Npgsql;
using NpgsqlTypes;

using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("booking")]
    public class BookingController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;
        private readonly IEmailService _emailService;

        public BookingController(PostgresSqlRunner sql, IEmailService emailService)
        {
            _sql = sql;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost("lock-seat")]
        public async Task<IActionResult> LockSeats([FromBody] LockSeatRequest request)
        {
            Console.WriteLine($"[Booking/LockSeats] Request received tripId={request.TripId}, seats={string.Join(",", request.SeatNumbers)}");
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
                        UNION
                        SELECT tr.""SeatNumber"" AS seat_number
                        FROM ""Travelers"" tr
                        INNER JOIN ""Bookings"" bk ON bk.""Id"" = tr.""BookingId""
                        WHERE bk.""TripId"" = @tripId
                          AND bk.""Status"" = 'Confirmed'
                          AND tr.""SeatNumber"" = ANY(@seatNumbers)
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
                    return BadRequest($"Seat {conflict} is already unavailable");

                foreach (var seat in seatNumbers)
                {
                    await using var insertLock = _sql.CreateCommand(
                        connection,
                        @"INSERT INTO ""SeatLocks"" (""Id"", ""TripId"", ""SeatNumber"", ""LockedByUserId"", ""ExpiryTime"")
                          VALUES (@id, @tripId, @seatNumber, @lockedByUserId, @expiryTime);",
                        transaction,
                        new NpgsqlParameter("id", Guid.NewGuid()),
                        new NpgsqlParameter("tripId", request.TripId),
                        new NpgsqlParameter("seatNumber", seat),
                        new NpgsqlParameter("lockedByUserId", userId),
                        new NpgsqlParameter("expiryTime", DateTime.UtcNow.AddMinutes(5)));

                    await insertLock.ExecuteNonQueryAsync();
                }

                return Ok("Seats locked");
            });
        }

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            Console.WriteLine($"[Booking/CreateBooking] Request received tripId={request.TripId}, travelers={request.Travelers.Count}");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);
            var seatNumbers = request.Travelers.Select(t => t.SeatNumber).Distinct().ToArray();

            return await _sql.WithTransactionAsync<IActionResult>(async (connection, transaction) =>
            {
                await using var tripCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT ""Price""
                      FROM ""Trips""
                      WHERE ""Id"" = @tripId
                      LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("tripId", request.TripId));

                var tripPriceObject = await tripCommand.ExecuteScalarAsync();
                if (tripPriceObject == null)
                    return BadRequest("Invalid trip");

                var tripPrice = Convert.ToDecimal(tripPriceObject);

                await using var lockCountCommand = _sql.CreateCommand(
                    connection,
                    @"
                    SELECT COUNT(DISTINCT ""SeatNumber"")
                    FROM ""SeatLocks""
                    WHERE ""TripId"" = @tripId
                      AND ""LockedByUserId"" = @userId
                      AND ""ExpiryTime"" > @nowUtc
                      AND ""SeatNumber"" = ANY(@seatNumbers);",
                    transaction,
                    new NpgsqlParameter("tripId", request.TripId),
                    new NpgsqlParameter("userId", userId),
                    new NpgsqlParameter("nowUtc", DateTime.UtcNow),
                    new NpgsqlParameter<string[]>("seatNumbers", seatNumbers)
                    {
                        NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Text
                    });

                var validLockCount = Convert.ToInt32(await lockCountCommand.ExecuteScalarAsync());
                if (validLockCount != seatNumbers.Length)
                    return BadRequest("One or more seats are not locked by this user");

                // Single lady booking validation
                if (request.IsSingleLady)
                {
                    var nonFemale = request.Travelers.FirstOrDefault(t => t.Gender != "Female");
                    if (nonFemale != null)
                        return BadRequest("Single lady booking requires all travelers to be female");
                }

                // Female seat rule validation
                await using var layoutCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT s.""SeatNumber"", s.""Row"", s.""Column""
                      FROM ""Seats"" s
                      INNER JOIN ""Buses"" b ON b.""LayoutId"" = s.""LayoutId""
                      INNER JOIN ""Trips"" t ON t.""BusId"" = b.""Id""
                      WHERE t.""Id"" = @tripId;",
                    transaction,
                    new NpgsqlParameter("tripId", request.TripId));

                var layoutSeats = new List<(string SeatNumber, int Row, int Column)>();
                await using (var reader = await layoutCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        layoutSeats.Add((
                            reader.GetString(0),
                            reader.GetInt32(1),
                            reader.GetInt32(2)
                        ));
                    }
                }

                await using var travelersCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT tr.""SeatNumber"", tr.""Gender""
                      FROM ""Travelers"" tr
                      INNER JOIN ""Bookings"" bk ON bk.""Id"" = tr.""BookingId""
                      WHERE bk.""TripId"" = @tripId AND bk.""Status"" = 'Confirmed';",
                    transaction,
                    new NpgsqlParameter("tripId", request.TripId));

                var existingTravelers = new Dictionary<string, string>();
                await using (var reader = await travelersCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        existingTravelers[reader.GetString(0)] = reader.GetString(1);
                    }
                }

                foreach (var t in request.Travelers)
                {
                    var seat = layoutSeats.FirstOrDefault(s => s.SeatNumber == t.SeatNumber);
                    if (seat.SeatNumber != null)
                    {
                        var adjacentSeats = layoutSeats.Where(s => s.Row == seat.Row && Math.Abs(s.Column - seat.Column) == 1);
                        foreach (var adj in adjacentSeats)
                        {
                            if (existingTravelers.TryGetValue(adj.SeatNumber, out var adjGender))
                            {
                                if ((adjGender == "Female" && t.Gender == "Male") ||
                                    (adjGender == "Male" && t.Gender == "Female"))
                                {
                                    return BadRequest($"Gender constraint violation: Seat {t.SeatNumber} cannot be booked by a {t.Gender} as adjacent seat {adj.SeatNumber} is booked by a {adjGender}.");
                                }
                            }
                        }
                    }
                }

                await using var settingCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT ""Value"" FROM ""Settings"" WHERE ""Key"" = 'PlatformFeePercentage' LIMIT 1;",
                    transaction);

                var feeSetting = await settingCommand.ExecuteScalarAsync();
                decimal platformFeePercentage = feeSetting != null ? decimal.Parse((string)feeSetting) : 10m; // Default 10%

                var platformFee = request.Travelers.Count * (tripPrice * platformFeePercentage / 100m);

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TripId = request.TripId,
                    TotalAmount = (tripPrice * request.Travelers.Count) + platformFee,
                    PlatformFee = platformFee,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                await using var insertBooking = _sql.CreateCommand(
                    connection,
                    @"INSERT INTO ""Bookings"" (""Id"", ""UserId"", ""TripId"", ""TotalAmount"", ""PlatformFee"", ""Status"", ""CreatedAt"")
                      VALUES (@id, @userId, @tripId, @totalAmount, @platformFee, @status, @createdAt);",
                    transaction,
                    new NpgsqlParameter("id", booking.Id),
                    new NpgsqlParameter("userId", booking.UserId),
                    new NpgsqlParameter("tripId", booking.TripId),
                    new NpgsqlParameter("totalAmount", booking.TotalAmount),
                    new NpgsqlParameter("platformFee", booking.PlatformFee),
                    new NpgsqlParameter("status", booking.Status),
                    new NpgsqlParameter("createdAt", booking.CreatedAt));

                await insertBooking.ExecuteNonQueryAsync();

                foreach (var traveler in request.Travelers)
                {
                    traveler.Id = Guid.NewGuid();
                    traveler.BookingId = booking.Id;

                    await using var insertTraveler = _sql.CreateCommand(
                        connection,
                        @"INSERT INTO ""Travelers"" (""Id"", ""BookingId"", ""Name"", ""Age"", ""Gender"", ""SeatNumber"")
                          VALUES (@id, @bookingId, @name, @age, @gender, @seatNumber);",
                        transaction,
                        new NpgsqlParameter("id", traveler.Id),
                        new NpgsqlParameter("bookingId", traveler.BookingId),
                        new NpgsqlParameter("name", traveler.Name),
                        new NpgsqlParameter("age", traveler.Age),
                        new NpgsqlParameter("gender", traveler.Gender),
                        new NpgsqlParameter("seatNumber", traveler.SeatNumber));

                    await insertTraveler.ExecuteNonQueryAsync();
                }

                await using var removeLocks = _sql.CreateCommand(
                    connection,
                    @"DELETE FROM ""SeatLocks""
                      WHERE ""TripId"" = @tripId
                        AND ""LockedByUserId"" = @userId;",
                    transaction,
                    new NpgsqlParameter("tripId", request.TripId),
                    new NpgsqlParameter("userId", userId));

                await removeLocks.ExecuteNonQueryAsync();

                return Ok(booking);
            });
        }

        [Authorize]
        [HttpPost("payment")]
        public async Task<IActionResult> Payment([FromQuery] Guid bookingId)
        {
            Console.WriteLine($"[Booking/Payment] Legacy booking payment endpoint called bookingId={bookingId}");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            return await _sql.WithTransactionAsync<IActionResult>(async (connection, transaction) =>
            {
                await using var bookingCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT ""Id"", ""UserId"", ""TripId"", ""TotalAmount"", ""Status"", ""CreatedAt""
                      FROM ""Bookings""
                      WHERE ""Id"" = @bookingId AND ""UserId"" = @userId
                      LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId),
                    new NpgsqlParameter("userId", userId));

                await using var bookingReader = await bookingCommand.ExecuteReaderAsync();
                if (!await bookingReader.ReadAsync())
                    return BadRequest("Invalid booking");

                var totalAmount = bookingReader.GetDecimal(bookingReader.GetOrdinal("TotalAmount"));
                await bookingReader.CloseAsync();

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

                // Send email
                await using var tripDetailsCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT u.""Email"", u.""Name"", r.""Source"", r.""Destination"", b.""BusNumber"", t.""PickupAddress"", t.""DropAddress""
                      FROM ""Bookings"" bk
                      INNER JOIN ""Users"" u ON u.""Id"" = bk.""UserId""
                      INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                      INNER JOIN ""Routes"" r ON r.""Id"" = t.""RouteId""
                      INNER JOIN ""Buses"" b ON b.""Id"" = t.""BusId""
                      WHERE bk.""Id"" = @bookingId LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("bookingId", bookingId));

                string email = "", name = "", source = "", dest = "", busNumber = "", pickup = "", drop = "";
                await using (var reader = await tripDetailsCommand.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        email = reader.GetString(0);
                        name = reader.GetString(1);
                        source = reader.GetString(2);
                        dest = reader.GetString(3);
                        busNumber = reader.GetString(4);
                        pickup = reader.GetString(5);
                        drop = reader.GetString(6);
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

                if (!string.IsNullOrEmpty(email))
                {
                    var routeStr = $"{source} to {dest} (Bus: {busNumber}). Pickup: {pickup}, Drop: {drop}";
                    var seatsStr = string.Join(", ", seatsList);
                    await _emailService.SendBookingConfirmationAsync(email, name, bookingId.ToString(), routeStr, seatsStr);
                }

                return Ok("Payment success");
            });
        }
    }
}

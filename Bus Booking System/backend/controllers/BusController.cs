using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using Npgsql;
using backend.Services;

namespace backend.Controllers
{
    [ApiController]
    [Route("bus")]
    public class BusController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public BusController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [Authorize(Roles = "Operator")]
        [HttpPost("/bus")]
        public async Task<IActionResult> AddBus([FromBody] Bus bus)
        {
            Console.WriteLine($"[Bus/AddBus] Request received for busNumber={bus.BusNumber}");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            var op = await _sql.QuerySingleOrDefaultAsync(
                @"SELECT ""Id"", ""UserId"", ""Approved"", ""IsActive"", ""Address""
                  FROM ""Operators""
                  WHERE ""UserId"" = @userId AND ""Approved"" = TRUE
                  LIMIT 1;",
                reader => new Operator
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                    Approved = reader.GetBoolean(reader.GetOrdinal("Approved")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    Address = reader.GetString(reader.GetOrdinal("Address"))
                },
                new NpgsqlParameter("userId", userId));

            if (op == null)
                return BadRequest("Operator not approved");

            bus.Id = Guid.NewGuid();
            bus.OperatorId = op.Id;

            await _sql.ExecuteAsync(
                @"INSERT INTO ""Buses"" (""Id"", ""OperatorId"", ""BusNumber"", ""TotalSeats"", ""LayoutId"", ""IsActive"")
                  VALUES (@id, @operatorId, @busNumber, @totalSeats, @layoutId, @isActive);",
                new NpgsqlParameter("id", bus.Id),
                new NpgsqlParameter("operatorId", bus.OperatorId),
                new NpgsqlParameter("busNumber", bus.BusNumber),
                new NpgsqlParameter("totalSeats", bus.TotalSeats),
                new NpgsqlParameter("layoutId", bus.LayoutId),
                new NpgsqlParameter("isActive", bus.IsActive));

            Console.WriteLine($"[Bus/AddBus] Bus created with id={bus.Id}, operatorId={bus.OperatorId}");
            return Ok(bus);
        }

        [HttpGet]
        public async Task<IActionResult> GetBuses()
        {
            Console.WriteLine("[Bus/GetBuses] Fetching all buses");
            var buses = await _sql.QueryAsync(
                @"SELECT ""Id"", ""OperatorId"", ""BusNumber"", ""TotalSeats"", ""LayoutId"", ""IsActive""
                  FROM ""Buses""
                  ORDER BY ""BusNumber"";",
                reader => new Bus
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    OperatorId = reader.GetGuid(reader.GetOrdinal("OperatorId")),
                    BusNumber = reader.GetString(reader.GetOrdinal("BusNumber")),
                    TotalSeats = reader.GetInt32(reader.GetOrdinal("TotalSeats")),
                    LayoutId = reader.GetGuid(reader.GetOrdinal("LayoutId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                });

            return Ok(buses);
        }

        [Authorize(Roles = "Operator")]
        [HttpGet("my-buses")]
        public async Task<IActionResult> GetMyBuses()
        {
            Console.WriteLine("[Bus/GetMyBuses] Fetching operator's buses");
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            var buses = await _sql.QueryAsync(
                @"SELECT b.""Id"", b.""OperatorId"", b.""BusNumber"", b.""TotalSeats"", b.""LayoutId"", b.""IsActive""
                  FROM ""Buses"" b
                  INNER JOIN ""Operators"" o ON o.""Id"" = b.""OperatorId""
                  WHERE o.""UserId"" = @userId
                  ORDER BY b.""BusNumber"";",
                reader => new Bus
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    OperatorId = reader.GetGuid(reader.GetOrdinal("OperatorId")),
                    BusNumber = reader.GetString(reader.GetOrdinal("BusNumber")),
                    TotalSeats = reader.GetInt32(reader.GetOrdinal("TotalSeats")),
                    LayoutId = reader.GetGuid(reader.GetOrdinal("LayoutId")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                },
                new NpgsqlParameter("userId", userId));

            return Ok(buses);
        }

        [Authorize(Roles = "Operator")]
        [HttpPut("{busId}/toggle")]
        public async Task<IActionResult> ToggleBus(Guid busId, [FromServices] IEmailService emailService)
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized("UserId claim missing");

            var userId = Guid.Parse(userIdClaim);

            return await _sql.WithTransactionAsync<IActionResult>(async (connection, transaction) =>
            {
                // Verify operator owns this bus
                await using var checkCommand = _sql.CreateCommand(
                    connection,
                    @"SELECT b.""IsActive"", o.""Id""
                      FROM ""Buses"" b
                      INNER JOIN ""Operators"" o ON o.""Id"" = b.""OperatorId""
                      WHERE b.""Id"" = @busId AND o.""UserId"" = @userId
                      LIMIT 1;",
                    transaction,
                    new NpgsqlParameter("busId", busId),
                    new NpgsqlParameter("userId", userId));

                bool isActive;
                await using (var reader = await checkCommand.ExecuteReaderAsync())
                {
                    if (!await reader.ReadAsync())
                        return Unauthorized("Bus not found or you do not own it");
                    isActive = reader.GetBoolean(0);
                }

                var newStatus = !isActive;

                await using var updateCommand = _sql.CreateCommand(
                    connection,
                    @"UPDATE ""Buses"" SET ""IsActive"" = @newStatus WHERE ""Id"" = @busId;",
                    transaction,
                    new NpgsqlParameter("newStatus", newStatus),
                    new NpgsqlParameter("busId", busId));

                await updateCommand.ExecuteNonQueryAsync();

                if (!newStatus)
                {
                    // Cancel all future trips' bookings for this bus
                    await using var tripsCommand = _sql.CreateCommand(
                        connection,
                        @"SELECT bk.""Id"", bk.""TotalAmount"", u.""Email"", u.""Name""
                          FROM ""Bookings"" bk
                          INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                          INNER JOIN ""Users"" u ON u.""Id"" = bk.""UserId""
                          WHERE t.""BusId"" = @busId
                            AND t.""JourneyDate"" >= CURRENT_DATE
                            AND bk.""Status"" = 'Confirmed';",
                        transaction,
                        new NpgsqlParameter("busId", busId));

                    var bookingsToCancel = new List<(Guid Id, decimal Amount, string Email, string Name)>();
                    await using (var reader = await tripsCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            bookingsToCancel.Add((
                                reader.GetGuid(0),
                                reader.GetDecimal(1),
                                reader.GetString(2),
                                reader.GetString(3)
                            ));
                        }
                    }

                    foreach (var bk in bookingsToCancel)
                    {
                        await using var cancelCmd = _sql.CreateCommand(
                            connection,
                            @"UPDATE ""Bookings"" SET ""Status"" = 'Cancelled' WHERE ""Id"" = @id;",
                            transaction,
                            new NpgsqlParameter("id", bk.Id));
                        await cancelCmd.ExecuteNonQueryAsync();

                        await using var insertCancellation = _sql.CreateCommand(
                            connection,
                            @"INSERT INTO ""Cancellations"" (""Id"", ""BookingId"", ""RefundAmount"", ""CancelledAt"")
                              VALUES (@id, @bookingId, @refundAmount, @cancelledAt);",
                            transaction,
                            new NpgsqlParameter("id", Guid.NewGuid()),
                            new NpgsqlParameter("bookingId", bk.Id),
                            new NpgsqlParameter("refundAmount", bk.Amount), // 100% refund when operator cancels
                            new NpgsqlParameter("cancelledAt", DateTime.UtcNow));
                        await insertCancellation.ExecuteNonQueryAsync();

                        await emailService.SendBookingCancellationAsync(bk.Email, bk.Name, bk.Id.ToString(), bk.Amount);
                    }
                }

                return Ok(new { isActive = newStatus });
            });
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using backend.DTOs;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("trip")]
    public class TripController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public TripController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [Authorize(Roles = "Operator,Admin")]
        [HttpPost("/trip")]
        public async Task<IActionResult> CreateTrip([FromBody] Trip trip)
        {
            Console.WriteLine($"[Trip/CreateTrip] Request received busId={trip.BusId}, routeId={trip.RouteId}, journeyDate={trip.JourneyDate:O}");
            var busExists = await _sql.ExecuteScalarAsync(
                @"SELECT 1
                  FROM ""Buses""
                  WHERE ""Id"" = @busId
                  LIMIT 1;",
                new NpgsqlParameter("busId", trip.BusId));

            if (busExists == null)
                return BadRequest("Invalid bus");

            var route = await _sql.QuerySingleOrDefaultAsync(
                @"SELECT ""Distance"" FROM ""Routes"" WHERE ""Id"" = @routeId LIMIT 1;",
                reader => reader.GetDouble(0),
                new NpgsqlParameter("routeId", trip.RouteId));

            if (route == null) return BadRequest("Invalid route");
            if (route == 0) return BadRequest("Route distance is 0");

            double distance = route;
            double hours = (trip.ArrivalTime - trip.DepartureTime).TotalHours;
            if (hours < 0) hours += 24; // Handle cross-midnight trips

            double maxHours = distance / 20.0;
            double minHours = distance / 120.0;

            if (hours < minHours || hours > maxHours)
                return BadRequest($"Invalid trip duration ({hours:F1} hours). For {distance} km, must be between {minHours:F1} and {maxHours:F1} hours (20-120 km/h).");

            if (string.IsNullOrWhiteSpace(trip.PickupAddress) || string.IsNullOrWhiteSpace(trip.DropAddress))
                return BadRequest("Pickup and Drop addresses are mandatory.");

            trip.Id = Guid.NewGuid();

            await _sql.ExecuteAsync(
                @"INSERT INTO ""Trips"" (""Id"", ""BusId"", ""RouteId"", ""JourneyDate"", ""DepartureTime"", ""ArrivalTime"", ""Price"", ""PickupAddress"", ""DropAddress"")
                  VALUES (@id, @busId, @routeId, @journeyDate, @departureTime, @arrivalTime, @price, @pickupAddress, @dropAddress);",
                new NpgsqlParameter("id", trip.Id),
                new NpgsqlParameter("busId", trip.BusId),
                new NpgsqlParameter("routeId", trip.RouteId),
                new NpgsqlParameter("journeyDate", trip.JourneyDate),
                new NpgsqlParameter("departureTime", trip.DepartureTime),
                new NpgsqlParameter("arrivalTime", trip.ArrivalTime),
                new NpgsqlParameter("price", trip.Price),
                new NpgsqlParameter("pickupAddress", trip.PickupAddress),
                new NpgsqlParameter("dropAddress", trip.DropAddress));

            Console.WriteLine($"[Trip/CreateTrip] Trip created with id={trip.Id}");
            return Ok(trip);
        }

        [HttpGet("{tripId:guid}")]
        public async Task<IActionResult> GetTripDetails(Guid tripId)
        {
            Console.WriteLine($"[Trip/GetTripDetails] Fetching tripId={tripId}");
            var trip = await _sql.QuerySingleOrDefaultAsync(
                BuildTripSearchSql("AND t.\"Id\" = @tripId"),
                MapTripSearchResponse,
                new NpgsqlParameter("nowUtc", DateTime.UtcNow),
                new NpgsqlParameter("tripId", tripId));

            return trip == null ? NotFound("Trip not found") : Ok(trip);
        }

        [HttpGet("/trip-search")]
        public async Task<IActionResult> Search([FromQuery] string source, [FromQuery] string destination, [FromQuery] DateTime date)
        {
            Console.WriteLine($"[Trip/Search] Searching trips source={source}, destination={destination}, date={date:yyyy-MM-dd}");
            var trips = await _sql.QueryAsync(
                BuildTripSearchSql(
                    @"AND lower(r.""Source"") LIKE @source
                      AND lower(r.""Destination"") LIKE @destination
                      AND t.""JourneyDate""::date = @journeyDate"),
                MapTripSearchResponse,
                new NpgsqlParameter("nowUtc", DateTime.UtcNow),
                new NpgsqlParameter("source", $"%{source.Trim().ToLower()}%"),
                new NpgsqlParameter("destination", $"%{destination.Trim().ToLower()}%"),
                new NpgsqlParameter("journeyDate", date.Date));
            Console.WriteLine($"[Trip/Search] Found {trips.Count} trips for source={source}, destination={destination}, date={date:yyyy-MM-dd}");
            return Ok(trips);
        }

        private static string BuildTripSearchSql(string filters)
        {
            return $@"
                SELECT
                    t.""Id"",
                    t.""BusId"",
                    t.""RouteId"",
                    b.""LayoutId"",
                    b.""BusNumber"",
                    b.""TotalSeats"",
                    GREATEST(
                        b.""TotalSeats""
                        - COALESCE(booked_stats.booked_count, 0)
                        - COALESCE(lock_stats.locked_count, 0),
                        0
                    ) AS ""AvailableSeats"",
                    r.""Source"",
                    r.""Destination"",
                    r.""Distance"",
                    t.""JourneyDate"",
                    t.""DepartureTime"",
                    t.""ArrivalTime"",
                    CAST(EXTRACT(EPOCH FROM (t.""ArrivalTime"" - t.""DepartureTime"")) / 60 AS INTEGER) AS ""TravelMinutes"",
                    t.""Price"",
                    t.""PickupAddress"",
                    t.""DropAddress"",
                    COALESCE(booked_stats.booked_seat_numbers, ARRAY[]::text[]) AS ""BookedSeatNumbers"",
                    COALESCE(lock_stats.locked_seat_numbers, ARRAY[]::text[]) AS ""LockedSeatNumbers"",
                    COALESCE(booked_stats.female_booked_seat_numbers, ARRAY[]::text[]) AS ""FemaleBookedSeatNumbers""
                FROM ""Trips"" t
                INNER JOIN ""Routes"" r ON r.""Id"" = t.""RouteId""
                INNER JOIN ""Buses"" b ON b.""Id"" = t.""BusId""
                LEFT JOIN (
                    SELECT
                        bk.""TripId"",
                        COUNT(DISTINCT tr.""SeatNumber"") AS booked_count,
                        ARRAY_AGG(DISTINCT tr.""SeatNumber"") AS booked_seat_numbers,
                        ARRAY_REMOVE(
                            ARRAY_AGG(DISTINCT CASE WHEN tr.""Gender"" = 'Female' THEN tr.""SeatNumber"" END),
                            NULL
                        ) AS female_booked_seat_numbers
                    FROM ""Bookings"" bk
                    INNER JOIN ""Travelers"" tr ON tr.""BookingId"" = bk.""Id""
                    WHERE bk.""Status"" = 'Confirmed'
                    GROUP BY bk.""TripId""
                ) booked_stats ON booked_stats.""TripId"" = t.""Id""
                LEFT JOIN (
                    SELECT
                        sl.""TripId"",
                        COUNT(DISTINCT sl.""SeatNumber"") AS locked_count,
                        ARRAY_AGG(DISTINCT sl.""SeatNumber"") AS locked_seat_numbers
                    FROM ""SeatLocks"" sl
                    WHERE sl.""ExpiryTime"" > @nowUtc
                    GROUP BY sl.""TripId""
                ) lock_stats ON lock_stats.""TripId"" = t.""Id""
                WHERE 1 = 1
                {filters}
                ORDER BY t.""DepartureTime"";";
        }

        private static TripSearchResponse MapTripSearchResponse(NpgsqlDataReader reader)
        {
            return new TripSearchResponse
            {
                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                BusId = reader.GetGuid(reader.GetOrdinal("BusId")),
                RouteId = reader.GetGuid(reader.GetOrdinal("RouteId")),
                LayoutId = reader.GetGuid(reader.GetOrdinal("LayoutId")),
                BusNumber = reader.GetString(reader.GetOrdinal("BusNumber")),
                TotalSeats = reader.GetInt32(reader.GetOrdinal("TotalSeats")),
                AvailableSeats = reader.GetInt32(reader.GetOrdinal("AvailableSeats")),
                Source = reader.GetString(reader.GetOrdinal("Source")),
                Destination = reader.GetString(reader.GetOrdinal("Destination")),
                Distance = reader.GetDouble(reader.GetOrdinal("Distance")),
                JourneyDate = reader.GetDateTime(reader.GetOrdinal("JourneyDate")),
                DepartureTime = reader.GetTimeSpan(reader.GetOrdinal("DepartureTime")),
                ArrivalTime = reader.GetTimeSpan(reader.GetOrdinal("ArrivalTime")),
                TravelMinutes = reader.GetInt32(reader.GetOrdinal("TravelMinutes")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                PickupAddress = reader.GetString(reader.GetOrdinal("PickupAddress")),
                DropAddress = reader.GetString(reader.GetOrdinal("DropAddress")),
                BookedSeatNumbers = reader.GetFieldValue<string[]>(reader.GetOrdinal("BookedSeatNumbers")).ToList(),
                LockedSeatNumbers = reader.GetFieldValue<string[]>(reader.GetOrdinal("LockedSeatNumbers")).ToList(),
                FemaleBookedSeatNumbers = reader.GetFieldValue<string[]>(reader.GetOrdinal("FemaleBookedSeatNumbers")).ToList()
            };
        }
    }
}

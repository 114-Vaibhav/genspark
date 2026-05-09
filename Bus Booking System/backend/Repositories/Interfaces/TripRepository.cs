using backend.Data;
using backend.Models;
using backend.DTOs;
using backend.Repositories.Interfaces;
using Npgsql;

namespace backend.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly PostgresSqlRunner _sql;

        public TripRepository(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        public async Task<bool> BusExistsAsync(Guid busId)
        {
            var result = await _sql.ExecuteScalarAsync(
                @"SELECT 1
                  FROM ""Buses""
                  WHERE ""Id""=@busId
                  LIMIT 1;",
                new NpgsqlParameter("busId", busId));

            return result != null;
        }

        public async Task<double?> GetRouteDistanceAsync(Guid routeId)
        {
            return await _sql.QuerySingleOrDefaultAsync(
                @"SELECT ""Distance""
                FROM ""Routes""
                WHERE ""Id""=@routeId
                LIMIT 1;",
                reader => reader.GetDouble(0),
                new NpgsqlParameter("routeId", routeId));
        }

        public async Task CreateTripAsync(Trip trip)
        {
            await _sql.ExecuteAsync(
                @"INSERT INTO ""Trips""
                (""Id"", ""BusId"", ""RouteId"", ""JourneyDate"",
                 ""DepartureTime"", ""ArrivalTime"", ""Price"",
                 ""PickupAddress"", ""DropAddress"")
                VALUES
                (@id,@busId,@routeId,@journeyDate,
                 @departureTime,@arrivalTime,@price,
                 @pickupAddress,@dropAddress);",

                new NpgsqlParameter("id", trip.Id),
                new NpgsqlParameter("busId", trip.BusId),
                new NpgsqlParameter("routeId", trip.RouteId),
                new NpgsqlParameter("journeyDate", trip.JourneyDate),
                new NpgsqlParameter("departureTime", trip.DepartureTime),
                new NpgsqlParameter("arrivalTime", trip.ArrivalTime),
                new NpgsqlParameter("price", trip.Price),
                new NpgsqlParameter("pickupAddress", trip.PickupAddress),
                new NpgsqlParameter("dropAddress", trip.DropAddress));
        }

        public async Task<TripSearchResponse?> GetTripByIdAsync(Guid tripId)
        {
            return await _sql.QuerySingleOrDefaultAsync(
                BuildSearchSql(@"AND t.""Id""=@tripId"),
                MapTrip,
                new NpgsqlParameter("nowUtc", DateTime.UtcNow),
                new NpgsqlParameter("tripId", tripId));
        }

        public async Task<List<TripSearchResponse>> SearchTripsAsync(
            string source,
            string destination,
            DateTime date)
        {
            return await _sql.QueryAsync(
                BuildSearchSql(
                @"AND lower(r.""Source"") LIKE @source
                  AND lower(r.""Destination"") LIKE @destination
                  AND t.""JourneyDate""::date=@journeyDate"),

                MapTrip,

                new NpgsqlParameter("nowUtc", DateTime.UtcNow),
                new NpgsqlParameter("source", $"%{source.ToLower()}%"),
                new NpgsqlParameter("destination", $"%{destination.ToLower()}%"),
                new NpgsqlParameter("journeyDate", date.Date));
        }

        private static string BuildSearchSql(string filters)
        {
            return $@"
                SELECT
                    t.""Id"",
                    t.""BusId"",
                    t.""RouteId"",
                    b.""LayoutId"",
                    b.""BusNumber"",
                    b.""TotalSeats"",
                    r.""Source"",
                    r.""Destination"",
                    r.""Distance"",
                    t.""JourneyDate"",
                    t.""DepartureTime"",
                    t.""ArrivalTime"",
                    t.""Price"",
                    t.""PickupAddress"",
                    t.""DropAddress""
                FROM ""Trips"" t
                INNER JOIN ""Routes"" r ON r.""Id"" = t.""RouteId""
                INNER JOIN ""Buses"" b ON b.""Id"" = t.""BusId""
                WHERE 1=1
                {filters}";
        }

        private static TripSearchResponse MapTrip(NpgsqlDataReader reader)
        {
            return new TripSearchResponse
            {
                Id = reader.GetGuid(0),
                BusId = reader.GetGuid(1),
                RouteId = reader.GetGuid(2),
                LayoutId = reader.GetGuid(3),
                BusNumber = reader.GetString(4),
                TotalSeats = reader.GetInt32(5),
                Source = reader.GetString(6),
                Destination = reader.GetString(7),
                Distance = reader.GetDouble(8),
                JourneyDate = reader.GetDateTime(9),
                DepartureTime = reader.GetTimeSpan(10),
                ArrivalTime = reader.GetTimeSpan(11),
                Price = reader.GetDecimal(12),
                PickupAddress = reader.GetString(13),
                DropAddress = reader.GetString(14)
            };
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using backend.Data;
using Microsoft.AspNetCore.Authorization;
using Npgsql;
using RouteEntity = backend.Models.Route;

namespace backend.Controllers
{
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public RouteController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [HttpGet("/routes")]
        public async Task<IActionResult> GetRoutes()
        {
            Console.WriteLine("[Route/GetRoutes] Fetching all routes");
            var routes = await _sql.QueryAsync(
                @"SELECT ""Id"", ""Source"", ""Destination"", ""Distance"", ""IsActive"", ""PickupPoints"", ""DropPoints""
                  FROM ""Routes""
                  ORDER BY ""Source"", ""Destination"";",
                reader => new RouteEntity
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Source = reader.GetString(reader.GetOrdinal("Source")),
                    Destination = reader.GetString(reader.GetOrdinal("Destination")),
                    Distance = reader.GetDouble(reader.GetOrdinal("Distance")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    PickupPoints = reader.GetFieldValue<string[]>(reader.GetOrdinal("PickupPoints")),
                    DropPoints = reader.GetFieldValue<string[]>(reader.GetOrdinal("DropPoints"))
                });

            return Ok(routes);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/routes")]
        public async Task<IActionResult> AddRoute([FromBody] RouteEntity route)
        {
            Console.WriteLine($"[Route/AddRoute] Request received source={route.Source}, destination={route.Destination}");
            
            if (route.Source.Trim().Equals(route.Destination.Trim(), StringComparison.OrdinalIgnoreCase))
                return BadRequest("Source and destination cannot be the same.");

            var exists = await _sql.ExecuteScalarAsync(
                @"SELECT 1
                  FROM ""Routes""
                  WHERE lower(""Source"") = lower(@source)
                    AND lower(""Destination"") = lower(@destination)
                  LIMIT 1;",
                new NpgsqlParameter("source", route.Source),
                new NpgsqlParameter("destination", route.Destination));

            if (exists != null)
                return BadRequest("Route already exists");

            route.Id = Guid.NewGuid();

            await _sql.ExecuteAsync(
                @"INSERT INTO ""Routes"" (""Id"", ""Source"", ""Destination"", ""Distance"", ""IsActive"", ""PickupPoints"", ""DropPoints"")
                  VALUES (@id, @source, @destination, @distance, @isActive, @pickupPoints, @dropPoints);",
                new NpgsqlParameter("id", route.Id),
                new NpgsqlParameter("source", route.Source),
                new NpgsqlParameter("destination", route.Destination),
                new NpgsqlParameter("distance", route.Distance),
                new NpgsqlParameter("isActive", true),
                new NpgsqlParameter("pickupPoints", route.PickupPoints ?? Array.Empty<string>()) { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text },
                new NpgsqlParameter("dropPoints", route.DropPoints ?? Array.Empty<string>()) { NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Text });

            Console.WriteLine($"[Route/AddRoute] Route created with id={route.Id}");
            return Ok(route);
        }

        [HttpGet("/route-search")]
        public async Task<IActionResult> Search([FromQuery] string source, [FromQuery] string destination)
        {
            Console.WriteLine($"[Route/Search] Searching routes source={source}, destination={destination}");
            var result = await _sql.QueryAsync(
                @"SELECT ""Id"", ""Source"", ""Destination"", ""Distance"", ""IsActive"", ""PickupPoints"", ""DropPoints""
                  FROM ""Routes""
                  WHERE lower(""Source"") LIKE @source
                    AND lower(""Destination"") LIKE @destination
                  ORDER BY ""Source"", ""Destination"";",
                reader => new RouteEntity
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    Source = reader.GetString(reader.GetOrdinal("Source")),
                    Destination = reader.GetString(reader.GetOrdinal("Destination")),
                    Distance = reader.GetDouble(reader.GetOrdinal("Distance")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                    PickupPoints = reader.GetFieldValue<string[]>(reader.GetOrdinal("PickupPoints")),
                    DropPoints = reader.GetFieldValue<string[]>(reader.GetOrdinal("DropPoints"))
                },
                new NpgsqlParameter("source", $"%{source.Trim().ToLower()}%"),
                new NpgsqlParameter("destination", $"%{destination.Trim().ToLower()}%"));

            return Ok(result);
        }
    }
}

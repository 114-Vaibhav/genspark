using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.DTOs;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public AdminController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [HttpGet("operators/pending")]
        public async Task<IActionResult> PendingOperators()
        {
            Console.WriteLine("[Admin/PendingOperators] Fetching pending operators");
            var data = await _sql.QueryAsync(
                @"SELECT
                      o.""Id"" AS ""OperatorId"",
                      u.""Id"" AS ""UserId"",
                      u.""Name"",
                      u.""Email"",
                      o.""Approved"",
                      o.""IsActive""
                  FROM ""Operators"" o
                  INNER JOIN ""Users"" u ON u.""Id"" = o.""UserId""
                  WHERE o.""Approved"" = FALSE
                  ORDER BY u.""Name"";",
                reader => new PendingOperatorResponse
                {
                    OperatorId = reader.GetGuid(reader.GetOrdinal("OperatorId")),
                    UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Approved = reader.GetBoolean(reader.GetOrdinal("Approved")),
                    IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                });

            return Ok(data);
        }

        [HttpPost("approve-operator/{operatorId}")]
        public async Task<IActionResult> ApproveOperator(Guid operatorId)
        {
            Console.WriteLine($"[Admin/ApproveOperator] Approving operatorId={operatorId}");
            var affectedRows = await _sql.ExecuteAsync(
                @"UPDATE ""Operators""
                  SET ""Approved"" = TRUE, ""IsActive"" = TRUE
                  WHERE ""Id"" = @operatorId;",
                new NpgsqlParameter("operatorId", operatorId));

            return affectedRows == 0 ? BadRequest("Not found") : Ok("Operator approved");
        }

        [HttpPost("reject-operator/{operatorId}")]
        public async Task<IActionResult> RejectOperator(Guid operatorId)
        {
            Console.WriteLine($"[Admin/RejectOperator] Rejecting operatorId={operatorId}");
            var affectedRows = await _sql.ExecuteAsync(
                @"UPDATE ""Operators""
                  SET ""Approved"" = FALSE, ""IsActive"" = FALSE
                  WHERE ""Id"" = @operatorId;",
                new NpgsqlParameter("operatorId", operatorId));

            return affectedRows == 0 ? BadRequest("Not found") : Ok("Operator rejected");
        }

        [HttpPut("operator/{operatorId}/toggle")]
        public async Task<IActionResult> ToggleOperator(Guid operatorId)
        {
            Console.WriteLine($"[Admin/ToggleOperator] Toggling operatorId={operatorId}");
            var affectedRows = await _sql.ExecuteAsync(
                @"UPDATE ""Operators""
                  SET ""IsActive"" = NOT ""IsActive""
                  WHERE ""Id"" = @operatorId;",
                new NpgsqlParameter("operatorId", operatorId));

            return affectedRows == 0 ? BadRequest("Not found") : Ok("Operator toggled");
        }

        [HttpPut("routes/{routeId}/toggle")]
        public async Task<IActionResult> ToggleRoute(Guid routeId)
        {
            Console.WriteLine($"[Admin/ToggleRoute] Toggling routeId={routeId}");
            var affectedRows = await _sql.ExecuteAsync(
                @"UPDATE ""Routes""
                  SET ""IsActive"" = NOT ""IsActive""
                  WHERE ""Id"" = @routeId;",
                new NpgsqlParameter("routeId", routeId));

            return affectedRows == 0 ? BadRequest("Not found") : Ok("Route toggled");
        }

        [HttpGet("revenue")]
        public async Task<IActionResult> Revenue()
        {
            Console.WriteLine("[Admin/Revenue] Fetching revenue grouped by operator");
            var data = await _sql.QueryAsync(
                @"SELECT
                      b.""OperatorId"",
                      SUM(bk.""TotalAmount"") AS ""Revenue""
                  FROM ""Bookings"" bk
                  INNER JOIN ""Trips"" t ON t.""Id"" = bk.""TripId""
                  INNER JOIN ""Buses"" b ON b.""Id"" = t.""BusId""
                  WHERE bk.""Status"" = 'Confirmed'
                  GROUP BY b.""OperatorId""
                  ORDER BY b.""OperatorId"";",
                reader => new
                {
                    OperatorId = reader.GetGuid(reader.GetOrdinal("OperatorId")),
                    Revenue = reader.GetDecimal(reader.GetOrdinal("Revenue"))
                });

            return Ok(data);
        }
        [HttpGet("settings/platform-fee")]
        public async Task<IActionResult> GetPlatformFee()
        {
            var value = await _sql.ExecuteScalarAsync(
                @"SELECT ""Value"" FROM ""Settings"" WHERE ""Key"" = 'PlatformFeePercentage' LIMIT 1;");
            
            if (value == null) return Ok(new { platformFeePercentage = 10 }); // Default 10%
            
            return Ok(new { platformFeePercentage = decimal.Parse((string)value) });
        }

        [HttpPut("settings/platform-fee")]
        public async Task<IActionResult> UpdatePlatformFee([FromBody] decimal platformFeePercentage)
        {
            var exists = await _sql.ExecuteScalarAsync(@"SELECT 1 FROM ""Settings"" WHERE ""Key"" = 'PlatformFeePercentage' LIMIT 1;");
            
            if (exists == null)
            {
                await _sql.ExecuteAsync(
                    @"INSERT INTO ""Settings"" (""Id"", ""Key"", ""Value"") VALUES (@id, 'PlatformFeePercentage', @val);",
                    new NpgsqlParameter("id", Guid.NewGuid()),
                    new NpgsqlParameter("val", platformFeePercentage.ToString()));
            }
            else
            {
                await _sql.ExecuteAsync(
                    @"UPDATE ""Settings"" SET ""Value"" = @val WHERE ""Key"" = 'PlatformFeePercentage';",
                    new NpgsqlParameter("val", platformFeePercentage.ToString()));
            }

            return Ok(new { message = "Platform fee updated" });
        }
    }
}

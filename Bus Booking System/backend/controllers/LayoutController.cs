using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using backend.Data;
using backend.Models;
using Npgsql;

namespace backend.Controllers
{
    [ApiController]
    [Route("layout")]
    public class LayoutController : ControllerBase
    {
        private readonly PostgresSqlRunner _sql;

        public LayoutController(PostgresSqlRunner sql)
        {
            _sql = sql;
        }

        [Authorize(Roles = "Admin,Operator")]
        [HttpPost]
        public async Task<IActionResult> CreateLayout([FromBody] SeatLayout layout)
        {
            Console.WriteLine($"[Layout/CreateLayout] Request received type={layout.BusType}, config={layout.Configuration}, totalSeats={layout.TotalSeats}");
            layout.Id = Guid.NewGuid();

            await _sql.ExecuteAsync(
                @"INSERT INTO ""SeatLayouts"" (""Id"", ""Name"", ""TotalSeats"", ""BusType"", ""Configuration"")
                  VALUES (@id, @name, @totalSeats, @busType, @configuration);",
                new NpgsqlParameter("id", layout.Id),
                new NpgsqlParameter("name", layout.Name),
                new NpgsqlParameter("totalSeats", layout.TotalSeats),
                new NpgsqlParameter("busType", layout.BusType ?? ""),
                new NpgsqlParameter("configuration", layout.Configuration ?? ""));

            // Auto-generate seats
            var parts = (layout.Configuration ?? "2+2").Split('+');
            int left = parts.Length > 0 && int.TryParse(parts[0], out int l) ? l : 2;
            int right = parts.Length > 1 && int.TryParse(parts[1], out int r) ? r : 2;
            int seatsPerRow = left + right;
            
            int totalRows = (int)Math.Ceiling((double)layout.TotalSeats / seatsPerRow);
            int seatCount = 1;

            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < left; col++)
                {
                    if (seatCount > layout.TotalSeats) break;
                    await InsertSeat(layout.Id, seatCount.ToString(), row, col);
                    seatCount++;
                }

                int rightStartCol = left + 1; // 1 column for aisle
                for (int col = rightStartCol; col < rightStartCol + right; col++)
                {
                    if (seatCount > layout.TotalSeats) break;
                    await InsertSeat(layout.Id, seatCount.ToString(), row, col);
                    seatCount++;
                }
            }

            Console.WriteLine($"[Layout/CreateLayout] Layout created with id={layout.Id} and {seatCount-1} seats");
            return Ok(layout);
        }

        private async Task InsertSeat(Guid layoutId, string seatNumber, int row, int column)
        {
            await _sql.ExecuteAsync(
                @"INSERT INTO ""Seats"" (""Id"", ""LayoutId"", ""SeatNumber"", ""Row"", ""Column"")
                  VALUES (@id, @layoutId, @seatNumber, @row, @column);",
                new NpgsqlParameter("id", Guid.NewGuid()),
                new NpgsqlParameter("layoutId", layoutId),
                new NpgsqlParameter("seatNumber", seatNumber),
                new NpgsqlParameter("row", row),
                new NpgsqlParameter("column", column));
        }

        [HttpGet("{layoutId}")]
        public async Task<IActionResult> GetLayout(Guid layoutId)
        {
            Console.WriteLine($"[Layout/GetLayout] Fetching layout seats for layoutId={layoutId}");
            var seats = await _sql.QueryAsync(
                @"SELECT ""Id"", ""LayoutId"", ""SeatNumber"", ""Row"", ""Column""
                  FROM ""Seats""
                  WHERE ""LayoutId"" = @layoutId
                  ORDER BY ""Row"", ""Column"", ""SeatNumber"";",
                reader => new Seat
                {
                    Id = reader.GetGuid(reader.GetOrdinal("Id")),
                    LayoutId = reader.GetGuid(reader.GetOrdinal("LayoutId")),
                    SeatNumber = reader.GetString(reader.GetOrdinal("SeatNumber")),
                    Row = reader.GetInt32(reader.GetOrdinal("Row")),
                    Column = reader.GetInt32(reader.GetOrdinal("Column"))
                },
                new NpgsqlParameter("layoutId", layoutId));

            return Ok(seats);
        }
    }
}

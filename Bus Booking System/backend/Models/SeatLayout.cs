
namespace backend.Models
{
    public class SeatLayout
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public string BusType { get; set; } = string.Empty; // e.g. "Seater", "Sleeper"
        public string Configuration { get; set; } = string.Empty; // e.g. "2+2", "1+2"
    }
}

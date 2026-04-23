
namespace backend.Models
{
    public class SeatLayout
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
    }
}

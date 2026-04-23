namespace backend.Models
{
    public class Seat
    {
        public Guid Id { get; set; }
        public Guid LayoutId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Column { get; set; }
    }
}

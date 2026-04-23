namespace backend.Models
{
    public class Booking
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid TripId { get; set; }

        public decimal TotalAmount { get; set; }
        public string Status { get; set; } // Pending / Confirmed

        public DateTime CreatedAt { get; set; }
    }
}
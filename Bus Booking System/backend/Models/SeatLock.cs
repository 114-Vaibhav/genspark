namespace backend.Models
{
    public class SeatLock
    {
        public Guid Id { get; set; }
        public Guid TripId { get; set; }

        public string SeatNumber { get; set; }
        public Guid LockedByUserId { get; set; }

        public DateTime ExpiryTime { get; set; }
    }
}
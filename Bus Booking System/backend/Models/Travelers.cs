namespace backend.Models
{



    public class Traveler
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }

        public string SeatNumber { get; set; }
    }
}
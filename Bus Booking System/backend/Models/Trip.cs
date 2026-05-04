namespace backend.Models
{
    public class Trip
    {
        public Guid Id { get; set; }

        public Guid BusId { get; set; }
        public Guid RouteId { get; set; }

        public DateTime JourneyDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }

        public decimal Price { get; set; }
        
        public string PickupAddress { get; set; } = string.Empty;
        public string DropAddress { get; set; } = string.Empty;
    }
}
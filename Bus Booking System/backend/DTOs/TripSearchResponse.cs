namespace backend.DTOs
{
    public class TripSearchResponse
    {
        public Guid Id { get; set; }
        public Guid BusId { get; set; }
        public Guid RouteId { get; set; }
        public Guid LayoutId { get; set; }
        public string BusNumber { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public double Distance { get; set; }
        public string PickupAddress { get; set; } = string.Empty;
        public string DropAddress { get; set; } = string.Empty;
        public DateTime JourneyDate { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public int TravelMinutes { get; set; }
        public decimal Price { get; set; }
        public List<string> BookedSeatNumbers { get; set; } = new();
        public List<string> LockedSeatNumbers { get; set; } = new();
        public List<string> FemaleBookedSeatNumbers { get; set; } = new();
    }
}

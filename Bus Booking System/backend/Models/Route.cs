namespace backend.Models
{
    public class Route
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public double Distance { get; set; }

        public bool IsActive { get; set; } = true;
        public string[] PickupPoints { get; set; } = Array.Empty<string>();
        public string[] DropPoints { get; set; } = Array.Empty<string>();
    }
}

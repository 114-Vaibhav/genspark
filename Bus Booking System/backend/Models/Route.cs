namespace backend.Models
{
    public class Route
    {
        public Guid Id { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public double Distance { get; set; }
    }
}

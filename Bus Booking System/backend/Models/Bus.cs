namespace backend.Models
{
    public class Bus
    {
        public Guid Id { get; set; }
        public Guid OperatorId { get; set; }

        public string BusNumber { get; set; } = string.Empty;
        public int TotalSeats { get; set; }

        public Guid LayoutId { get; set; }
        public bool IsActive { get; set; }
    }
}

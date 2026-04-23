namespace backend.Models
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }

        public decimal Amount { get; set; }
        public string Status { get; set; }

        public string PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
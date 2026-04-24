using System;

namespace backend.Models
{
    public class Cancellation
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }

        public decimal RefundAmount { get; set; }
        public DateTime CancelledAt { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace backend.DTOs
{
    public class TicketResponse
    {
        public Guid BookingId { get; set; }
        public string Status { get; set; }

        public List<string> Seats { get; set; }
        public decimal Amount { get; set; }

        public DateTime JourneyDate { get; set; }
        public string PickupAddress { get; set; }
        public string DropAddress { get; set; }
    }
}
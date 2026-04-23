using System;
using System.Collections.Generic;

namespace backend.DTOs
{
    public class LockSeatRequest
    {
        public Guid TripId { get; set; }
        public List<string> SeatNumbers { get; set; }
    }
}
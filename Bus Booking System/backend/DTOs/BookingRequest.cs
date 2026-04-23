using System;
using System.Collections.Generic;
using backend.Models;

namespace backend.DTOs
{
    public class BookingRequest
    {
        public Guid TripId { get; set; }
        public List<Traveler> Travelers { get; set; }
    }
}
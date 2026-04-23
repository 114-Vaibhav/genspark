using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;
using System.Security.Claims;

namespace backend.Controllers
{
    [ApiController]
    [Route("booking")]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }

        // 🔒 LOCK SEATS
        [Authorize]
        [HttpPost("lock-seat")]
        public async Task<IActionResult> LockSeats([FromBody] LockSeatRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("UserId").Value);

            foreach (var seat in request.SeatNumbers)
            {
                var existingLock = await _context.SeatLocks
                    .FirstOrDefaultAsync(s =>
                        s.TripId == request.TripId &&
                        s.SeatNumber == seat &&
                        s.ExpiryTime > DateTime.UtcNow);

                if (existingLock != null)
                    return BadRequest($"Seat {seat} already locked");

                _context.SeatLocks.Add(new SeatLock
                {
                    Id = Guid.NewGuid(),
                    TripId = request.TripId,
                    SeatNumber = seat,
                    LockedByUserId = userId,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5)
                });
            }

            await _context.SaveChangesAsync();

            return Ok("Seats locked");
        }

        // 🎟️ CREATE BOOKING
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("UserId").Value);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Validate locks
                foreach (var t in request.Travelers)
                {
                    var lockExists = await _context.SeatLocks.AnyAsync(l =>
                        l.TripId == request.TripId &&
                        l.SeatNumber == t.SeatNumber &&
                        l.LockedByUserId == userId &&
                        l.ExpiryTime > DateTime.UtcNow);

                    if (!lockExists)
                        return BadRequest($"Seat {t.SeatNumber} not locked");
                }

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    TripId = request.TripId,
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                foreach (var t in request.Travelers)
                {
                    t.Id = Guid.NewGuid();
                    t.BookingId = booking.Id;
                    _context.Travelers.Add(t);
                }

                await _context.SaveChangesAsync();

                // Remove locks
                var locks = _context.SeatLocks
                    .Where(l => l.TripId == request.TripId && l.LockedByUserId == userId);

                _context.SeatLocks.RemoveRange(locks);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(booking);
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Booking failed");
            }
        }

        // 💳 PAYMENT (DUMMY)
        [HttpPost("payment")]
        public async Task<IActionResult> Payment(Guid bookingId)
        {
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = bookingId,
                Amount = 500,
                Status = "Success",
                PaymentMethod = "Dummy",
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);

            var booking = await _context.Bookings.FindAsync(bookingId);
            booking.Status = "Confirmed";

            await _context.SaveChangesAsync();

            return Ok("Payment success");
        }
    }
}
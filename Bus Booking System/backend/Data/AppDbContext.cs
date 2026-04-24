using Microsoft.EntityFrameworkCore;
using backend.Models;
using RouteEntity = backend.Models.Route;

namespace backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<RouteEntity> Routes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Operator> Operators { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Trip> Trips { get; set; }

        public DbSet<SeatLayout> SeatLayouts { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<SeatLock> SeatLocks { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Traveler> Travelers { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }
    }
}
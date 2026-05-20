using Microsoft.EntityFrameworkCore;
using librarymanagement.ModelLib;

namespace librarymanagement.DataAccessLib.Contexts
{
    public class LibraryDbContext : DbContext
    {
        // Constructor required for AddDbContext
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .Property(b => b.BookId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<Member>()
                .Property(u => u.MemberId)
                .UseIdentityByDefaultColumn();
        }
    }
}
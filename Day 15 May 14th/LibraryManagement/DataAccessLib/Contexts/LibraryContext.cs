using Microsoft.EntityFrameworkCore;
using librarymanagementsystem.ModelLib;

namespace librarymanagementsystem.DataAccessLib.Contexts
{
    public class LibraryContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        public DbSet<BookCopies> BookCopies { get; set; }

        public DbSet<Stock> Stocks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserStat> UserStats { get; set; }

        public DbSet<MembershipTypes> MembershipTypes { get; set; }

        public DbSet<BorrowTransactions> BorrowTransactions { get; set; }

        public DbSet<BookReturnTransactions> BookReturnTransactions { get; set; }

        public DbSet<FineTransactions> FineTransactions { get; set; }
        public DbSet<Rules> Rules { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=librarymanagementsystem;Username=vaibhavgupta;Password=8098");
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .Property(b => b.BookId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<User>()
                .Property(u => u.UserId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<Stock>()
                .Property(s => s.StockId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<BookCopies>()
                .Property(bc => bc.BookCopiesId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<UserStat>()
                .Property(us => us.UserStatId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<MembershipTypes>()
                .Property(m => m.MembershipTypesId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<BorrowTransactions>()
                .Property(bt => bt.BorrowTransactionsId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<BookReturnTransactions>()
                .Property(br => br.BookReturnTransactionsId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<FineTransactions>()
                .Property(f => f.FineTransactionsId)
                .UseIdentityByDefaultColumn();

            modelBuilder.Entity<Rules>()
                .Property(r => r.RulesId)
                .UseIdentityByDefaultColumn();


            modelBuilder.Entity<Book>()
                .HasOne(b => b.Stock)
                .WithOne(s => s.Book)
                .HasForeignKey<Stock>(s => s.BookId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserStat)
                .WithOne(us => us.User)
                .HasForeignKey<UserStat>(us => us.UserId);
        }
    }
}
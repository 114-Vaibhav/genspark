using Microsoft.EntityFrameworkCore;
namespace simplenotification.Contexts
{
    public class NotificationDBContext : DbContext
    {
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=notificationnfcore;Username=vaibhavgupta;Password=8098");
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User> Users { get; set; }
        
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {

        //     modelBuilder.Entity<Customer>(c =>
        //     {
        //         c.HasKey(c => c.Id).HasName("PK_CustomerId");
        //         c.Property(c => c.DateOfBirth).HasColumnType("timestamp without time zone");
        //         //seeding
        //         c.HasData(new Customer() { Id = 101, Name = "Ramu", Phone = "9876543210", DateOfBirth= new DateTime(2000,12,12), Email = "ramu@gmail.com", Status = "Active" });
        //     });

        //     modelBuilder.Entity<Account>(a =>
        //     {
        //     a.HasKey(a => a.AccountNumber).HasName("PK_AccountNumber");

        //     a.HasOne(a => a.Customer)
        //     .WithMany(c => c.Accounts)
        //     .HasForeignKey(a => a.CustomerId)
        //     .HasConstraintName("FK_Account_Customer")
        //     .OnDelete(DeleteBehavior.Restrict);

        //     a.HasDiscriminator<string>("AccountType")
        //     .HasValue<SavingAccount>("Savings Account")
        //     .HasValue<CurrentAccount>("Current Account");




        //         a.HasData(new Account()
        //         {
        //             AccountNumber = "0009998877",
        //             Balance = 12343.4f,
        //             CustomerId = 101,
        //             Status = "Active"
        //         });
        //     });

        //     modelBuilder.Entity<SavingAccount>();
        //     modelBuilder.Entity<CurrentAccount>();

        // }
    
    

    }
    
}
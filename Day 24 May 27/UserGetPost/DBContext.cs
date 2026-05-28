using Microsoft.EntityFrameworkCore;
using UserGetPost.Models;
namespace UserGetPost.Context
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
    }
}
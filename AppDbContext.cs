using Microsoft.EntityFrameworkCore;
using BookStore.Models;

namespace BookStore.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
    }
}

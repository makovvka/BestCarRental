using BestCarRental.Models;
using Microsoft.EntityFrameworkCore;

namespace BestCarRental.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        { 

        }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Car>  Cars { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}

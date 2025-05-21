using Microsoft.EntityFrameworkCore;
using OrderingApp.WebApi.Domain.Models;

namespace OrderingApp.WebApi.Domain
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        /// <summary>
        /// For unit testing.
        /// </summary>
        public ApplicationDbContext()
        {
            
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderLine> OrderLines { get; set; }
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasData([new Customer {Id=1, Name = "Andrew", DateJoined = DateTime.UtcNow, Membership= CustomerMembership.None },
                new Customer {Id=2, Name = "John", DateJoined = DateTime.UtcNow.AddYears(-1).AddDays(-10), Membership= CustomerMembership.Gold },
                new Customer {Id=3, Name = "Ali", DateJoined = DateTime.UtcNow.AddYears(-2).AddDays(-2), Membership=CustomerMembership.Silver }]);
        }
    }
}

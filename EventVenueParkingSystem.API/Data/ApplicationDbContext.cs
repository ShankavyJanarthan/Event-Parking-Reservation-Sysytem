using EventParkingReservationSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Customers Table
        public DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(x => x.CustomerId);

                entity.Property(x => x.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(x => x.Phone)
                    .HasMaxLength(30);

                entity.Property(x => x.PasswordHash)
                    .IsRequired();

                entity.Property(x => x.Role)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(x => x.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                // Email must be unique
                entity.HasIndex(x => x.Email)
                    .IsUnique();
            });
        }
    }
}
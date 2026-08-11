using EventParkingReservationSystem.API.Helpers;
using EventParkingReservationSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            PasswordHelper passwordHelper)
        {
            // Ensure migrations are applied
            await context.Database.MigrateAsync();

            // Do not create duplicate admin
            var adminExists = await context.Customers
                .AnyAsync(x => x.Email == "admin@eventparking.local");

            if (adminExists)
            {
                return;
            }

            var admin = new Customer
            {
                FirstName = "System",
                LastName = "Administrator",

                Email = "admin@eventparking.local",

                Phone = "0000000000",

                PasswordHash =
                    passwordHelper.HashPassword("Admin123!"),

                Role = "Admin",

                Status = "Active",

                // Seed admin does not require email verification
                EmailVerified = true,

                EmailVerificationToken = null,
                EmailVerificationTokenExpiresAt = null,

                PasswordResetToken = null,
                PasswordResetTokenExpiresAt = null,

                CreatedAt = DateTime.UtcNow
            };

            await context.Customers.AddAsync(admin);

            await context.SaveChangesAsync();

            Console.WriteLine(
                "Development Admin account created: admin@eventparking.local");
        }
    }
}
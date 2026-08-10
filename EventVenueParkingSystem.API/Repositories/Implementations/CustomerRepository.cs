using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _context.Customers
                .FirstOrDefaultAsync(
                    x => x.Email.ToLower() == normalizedEmail);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLower();

            return await _context.Customers
                .AnyAsync(
                    x => x.Email.ToLower() == normalizedEmail);
        }

        public async Task<List<Customer>> SearchAsync(string? search)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var value = search.Trim().ToLower();

                query = query.Where(x =>
                    x.FirstName.ToLower().Contains(value) ||
                    x.LastName.ToLower().Contains(value) ||
                    x.Email.ToLower().Contains(value));
            }

            return await query
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Customer?> GetByVerificationTokenAsync(
    string hashedToken)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(
                    x => x.EmailVerificationToken == hashedToken);
        }

        public async Task<Customer?> GetByPasswordResetTokenAsync(
    string hashedToken)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(
                    x => x.PasswordResetToken == hashedToken);
        }
    }
}
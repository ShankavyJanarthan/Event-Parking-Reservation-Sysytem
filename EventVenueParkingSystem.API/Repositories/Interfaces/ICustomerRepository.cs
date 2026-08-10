using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int customerId);

        Task<Customer?> GetByEmailAsync(string email);

        Task<Customer?> GetByVerificationTokenAsync(string hashedToken);

        Task<Customer?> GetByPasswordResetTokenAsync(string hashedToken);

        Task<bool> EmailExistsAsync(string email);

        Task<List<Customer>> SearchAsync(string? search);

        Task AddAsync(Customer customer);

        Task UpdateAsync(Customer customer);

        Task SaveChangesAsync();
    }
}
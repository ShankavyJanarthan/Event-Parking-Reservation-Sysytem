using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(
            int paymentId);

        Task<Payment?> GetByBookingIdAsync(
            int bookingId);

        Task<bool> HasSuccessfulPaymentAsync(
            int bookingId);

        Task<bool> TransactionReferenceExistsAsync(
            string transactionReference);

        Task AddAsync(
            Payment payment);

        Task UpdateAsync(
            Payment payment);

        Task SaveChangesAsync();
    }
}
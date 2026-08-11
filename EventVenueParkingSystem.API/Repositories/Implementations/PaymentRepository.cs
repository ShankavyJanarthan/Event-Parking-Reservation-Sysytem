using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Payment By Id
        // =====================================================
        public async Task<Payment?> GetByIdAsync(
            int paymentId)
        {
            return await _context.Payments
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(
                    x => x.PaymentId == paymentId);
        }

        // =====================================================
        // Get Payment By Booking
        // =====================================================
        public async Task<Payment?> GetByBookingIdAsync(
            int bookingId)
        {
            return await _context.Payments
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(
                    x => x.BookingId == bookingId);
        }

        // =====================================================
        // Prevent Duplicate Successful Payment
        // =====================================================
        public async Task<bool> HasSuccessfulPaymentAsync(
            int bookingId)
        {
            return await _context.Payments
                .AnyAsync(x =>
                    x.BookingId == bookingId &&
                    x.Status == "Successful");
        }

        // =====================================================
        // Check Transaction Reference
        // =====================================================
        public async Task<bool> TransactionReferenceExistsAsync(
            string transactionReference)
        {
            return await _context.Payments
                .AnyAsync(x =>
                    x.TransactionReference ==
                    transactionReference);
        }

        // =====================================================
        // Add Payment
        // =====================================================
        public async Task AddAsync(
            Payment payment)
        {
            await _context.Payments
                .AddAsync(payment);
        }

        // =====================================================
        // Update Payment
        // =====================================================
        public Task UpdateAsync(
            Payment payment)
        {
            _context.Payments.Update(payment);

            return Task.CompletedTask;
        }

        // =====================================================
        // Save
        // =====================================================
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
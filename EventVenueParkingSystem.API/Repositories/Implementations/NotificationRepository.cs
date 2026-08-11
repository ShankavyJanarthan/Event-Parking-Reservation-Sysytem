using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Notification By Id
        // =====================================================
        public async Task<Notification?> GetByIdAsync(
            int notificationId)
        {
            return await _context.Notifications
                .Include(x => x.Customer)
                .Include(x => x.Booking)
                .FirstOrDefaultAsync(
                    x => x.NotificationId == notificationId);
        }

        // =====================================================
        // Get Customer Notifications
        // =====================================================
        public async Task<List<Notification>> GetByCustomerIdAsync(
            int customerId)
        {
            return await _context.Notifications
                .Include(x => x.Booking)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // =====================================================
        // Add Notification
        // =====================================================
        public async Task AddAsync(
            Notification notification)
        {
            await _context.Notifications
                .AddAsync(notification);
        }

        // =====================================================
        // Update Notification
        // =====================================================
        public Task UpdateAsync(
            Notification notification)
        {
            _context.Notifications.Update(
                notification);

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
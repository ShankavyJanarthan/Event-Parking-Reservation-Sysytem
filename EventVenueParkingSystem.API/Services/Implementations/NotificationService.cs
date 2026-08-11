using EventParkingReservationSystem.API.DTOs.Notifications;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository
            _notificationRepository;

        public NotificationService(
            INotificationRepository notificationRepository)
        {
            _notificationRepository =
                notificationRepository;
        }

        // =====================================================
        // Internal Notification Creation
        // =====================================================
        public async Task CreateAsync(
            int customerId,
            int? bookingId,
            string type,
            string title,
            string message)
        {
            var notification = new Notification
            {
                CustomerId =
                    customerId,

                BookingId =
                    bookingId,

                Type =
                    type,

                Title =
                    title,

                Message =
                    message,

                IsRead =
                    false,

                CreatedAt =
                    DateTime.UtcNow
            };

            await _notificationRepository.AddAsync(
                notification);

            await _notificationRepository
                .SaveChangesAsync();
        }

        // =====================================================
        // Get Customer Notifications
        // =====================================================
        public async Task<List<NotificationResponseDto>>
            GetCustomerNotificationsAsync(
                int customerId)
        {
            var notifications =
                await _notificationRepository
                    .GetByCustomerIdAsync(
                        customerId);

            return notifications
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Mark Notification As Read
        // =====================================================
        public async Task<string> MarkAsReadAsync(
            int notificationId,
            int customerId)
        {
            var notification =
                await _notificationRepository
                    .GetByIdAsync(
                        notificationId);

            if (notification == null)
            {
                throw new KeyNotFoundException(
                    "Notification was not found.");
            }

            if (notification.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to update this notification.");
            }

            if (notification.IsRead)
            {
                return "Notification is already marked as read.";
            }

            notification.IsRead =
                true;

            notification.ReadAt =
                DateTime.UtcNow;

            await _notificationRepository.UpdateAsync(
                notification);

            await _notificationRepository
                .SaveChangesAsync();

            return "Notification marked as read.";
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static NotificationResponseDto MapToResponse(
            Notification notification)
        {
            return new NotificationResponseDto
            {
                NotificationId =
                    notification.NotificationId,

                CustomerId =
                    notification.CustomerId,

                BookingId =
                    notification.BookingId,

                BookingNumber =
                    notification.Booking?.BookingNumber,

                Type =
                    notification.Type,

                Title =
                    notification.Title,

                Message =
                    notification.Message,

                IsRead =
                    notification.IsRead,

                ReadAt =
                    notification.ReadAt,

                CreatedAt =
                    notification.CreatedAt
            };
        }
    }
}
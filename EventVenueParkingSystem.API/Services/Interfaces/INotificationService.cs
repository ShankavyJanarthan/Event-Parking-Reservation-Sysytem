using EventParkingReservationSystem.API.DTOs.Notifications;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface INotificationService
    {
        Task CreateAsync(
            int customerId,
            int? bookingId,
            string type,
            string title,
            string message);

        Task<List<NotificationResponseDto>>
            GetCustomerNotificationsAsync(
                int customerId);

        Task<string> MarkAsReadAsync(
            int notificationId,
            int customerId);
    }
}
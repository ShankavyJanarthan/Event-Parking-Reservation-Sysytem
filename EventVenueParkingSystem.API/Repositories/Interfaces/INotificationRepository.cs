using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<Notification?> GetByIdAsync(
            int notificationId);

        Task<List<Notification>> GetByCustomerIdAsync(
            int customerId);

        Task AddAsync(
            Notification notification);

        Task UpdateAsync(
            Notification notification);

        Task SaveChangesAsync();
    }
}
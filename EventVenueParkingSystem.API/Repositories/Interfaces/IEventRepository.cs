using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(int eventId);

        Task<List<Event>> GetAllAsync(
            string? search,
            int? venueId,
            int? eventCategoryId);

        Task<bool> HasOverlappingEventAsync(
            int venueId,
            DateTime startDateTime,
            DateTime endDateTime,
            int? excludeEventId = null);

        Task AddAsync(Event eventItem);

        Task UpdateAsync(Event eventItem);

        Task DeleteAsync(Event eventItem);

        Task SaveChangesAsync();
    }
}
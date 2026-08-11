using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface IEventCategoryRepository
    {
        Task<EventCategory?> GetByIdAsync(int eventCategoryId);

        Task<EventCategory?> GetByNameAsync(string name);

        Task<List<EventCategory>> GetAllAsync();

        Task<bool> IsAssignedToEventsAsync(int eventCategoryId);

        Task AddAsync(EventCategory eventCategory);

        Task UpdateAsync(EventCategory eventCategory);

        Task DeleteAsync(EventCategory eventCategory);

        Task SaveChangesAsync();
    }
}

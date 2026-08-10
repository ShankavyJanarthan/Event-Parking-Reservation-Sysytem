using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface IVenueRepository
    {
        Task<Venue?> GetByIdAsync(int venueId);

        Task<List<Venue>> GetAllAsync();

        Task<bool> HasUpcomingEventsAsync(int venueId);

        Task AddAsync(Venue venue);

        Task UpdateAsync(Venue venue);

        Task DeleteAsync(Venue venue);

        Task SaveChangesAsync();
    }
}
using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface ISeatRepository
    {
        Task<Seat?> GetByIdAsync(int seatId);

        Task<List<Seat>> GetByEventIdAsync(int eventId);

        Task<int> GetSeatCountByEventAsync(int eventId);

        Task<bool> SeatExistsAsync(
            int eventId,
            string rowLabel,
            int seatNumber,
            string seatLabel,
            int? excludeSeatId = null);

        Task AddAsync(Seat seat);

        Task UpdateAsync(Seat seat);

        Task DeleteAsync(Seat seat);

        Task SaveChangesAsync();
    }
}
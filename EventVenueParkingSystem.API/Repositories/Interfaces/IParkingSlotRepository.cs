using EventParkingReservationSystem.API.Models;

namespace EventParkingReservationSystem.API.Repositories.Interfaces
{
    public interface IParkingSlotRepository
    {
        Task<ParkingSlot?> GetByIdAsync(int parkingSlotId);

        Task<List<ParkingSlot>> GetByEventIdAsync(int eventId);

        Task<bool> SlotExistsAsync(
            int eventId,
            string slotNumber,
            int? excludeParkingSlotId = null);

        Task AddAsync(ParkingSlot parkingSlot);

        Task UpdateAsync(ParkingSlot parkingSlot);

        Task DeleteAsync(ParkingSlot parkingSlot);

        Task SaveChangesAsync();
    }
}
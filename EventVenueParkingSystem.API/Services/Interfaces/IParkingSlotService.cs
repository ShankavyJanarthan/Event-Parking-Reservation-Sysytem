using EventParkingReservationSystem.API.DTOs.ParkingSlots;

namespace EventParkingReservationSystem.API.Services.Interfaces
{
    public interface IParkingSlotService
    {
        Task<List<ParkingSlotResponseDto>> GetByEventIdAsync(
            int eventId);

        Task<ParkingSlotResponseDto> GetByIdAsync(
            int parkingSlotId);

        Task<ParkingSlotResponseDto> CreateAsync(
            CreateParkingSlotDto dto);

        Task<ParkingSlotResponseDto> UpdateAsync(
            int parkingSlotId,
            UpdateParkingSlotDto dto);

        Task<string> DeleteAsync(
            int parkingSlotId);
    }
}
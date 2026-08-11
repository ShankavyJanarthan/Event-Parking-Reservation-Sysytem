namespace EventParkingReservationSystem.API.DTOs.ParkingSlots
{
    public class ParkingSlotResponseDto
    {
        public int ParkingSlotId { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string SlotNumber { get; set; } = string.Empty;

        public decimal Fee { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
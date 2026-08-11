namespace EventParkingReservationSystem.API.Models
{
    public class ParkingSlot
    {
        public int ParkingSlotId { get; set; }

        // Parking slot belongs to an event
        public int EventId { get; set; }

        public Event? Event { get; set; }

        // Example: P01, P02
        public string SlotNumber { get; set; } = string.Empty;

        // Parking fee for this slot
        public decimal Fee { get; set; }

        // Admin can enable / disable parking slot
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}

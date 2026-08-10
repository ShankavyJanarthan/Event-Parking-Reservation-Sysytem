namespace EventParkingReservationSystem.API.Models
{
    public class Seat
    {
        public int SeatId { get; set; }

        // =====================================================
        // Event
        // Seat belongs to a specific event
        // =====================================================
        public int EventId { get; set; }

        public Event? Event { get; set; }


        // =====================================================
        // Seat Information
        // Example: Row A, Seat 10
        // =====================================================
        public string RowLabel { get; set; } = string.Empty;

        public int SeatNumber { get; set; }


        // =====================================================
        // Display Label
        // Example: A10
        // =====================================================
        public string SeatLabel { get; set; } = string.Empty;


        // =====================================================
        // Seat Status
        // Active / Inactive
        // Booking availability will later be determined
        // using Booking/Reservation data.
        // =====================================================
        public bool IsActive { get; set; } = true;


        // =====================================================
        // Audit
        // =====================================================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
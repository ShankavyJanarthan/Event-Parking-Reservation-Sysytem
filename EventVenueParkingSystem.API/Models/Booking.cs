namespace EventParkingReservationSystem.API.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        // Example: BKG-2026-000001
        public string BookingNumber { get; set; } = string.Empty;

        // =====================================================
        // Customer
        // =====================================================
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }


        // =====================================================
        // Event
        // =====================================================
        public int EventId { get; set; }

        public Event? Event { get; set; }


        // =====================================================
        // Seat
        // =====================================================
        public int SeatId { get; set; }

        public Seat? Seat { get; set; }


        // =====================================================
        // Optional Parking
        // Maximum one parking slot per booking
        // =====================================================
        public int? ParkingSlotId { get; set; }

        public ParkingSlot? ParkingSlot { get; set; }


        // =====================================================
        // Price Snapshot
        // Keep booking-time prices even if prices change later
        // =====================================================
        public decimal SeatPrice { get; set; }

        public decimal ParkingFee { get; set; }

        public decimal TotalAmount { get; set; }


        // =====================================================
        // Status
        // Pending / Confirmed / Cancelled / Expired
        // =====================================================
        public string Status { get; set; } = "Pending";


        // =====================================================
        // Booking Hold
        // Default: 15 minutes
        // =====================================================
        public DateTime HoldExpiresAt { get; set; }


        // =====================================================
        // Audit
        // =====================================================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CancelledAt { get; set; }
    }
}
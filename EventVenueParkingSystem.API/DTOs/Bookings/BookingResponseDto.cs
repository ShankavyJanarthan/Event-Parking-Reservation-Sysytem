namespace EventParkingReservationSystem.API.DTOs.Bookings
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }

        public string BookingNumber { get; set; } = string.Empty;

        // =====================================================
        // Customer
        // =====================================================
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;


        // =====================================================
        // Event
        // =====================================================
        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;


        // =====================================================
        // Seat
        // =====================================================
        public int SeatId { get; set; }

        public string SeatLabel { get; set; } = string.Empty;


        // =====================================================
        // Optional Parking
        // =====================================================
        public int? ParkingSlotId { get; set; }

        public string? ParkingSlotNumber { get; set; }


        // =====================================================
        // Price Snapshot
        // =====================================================
        public decimal SeatPrice { get; set; }

        public decimal ParkingFee { get; set; }

        public decimal TotalAmount { get; set; }


        // =====================================================
        // Booking Status
        // Pending / Confirmed / Cancelled / Expired
        // =====================================================
        public string Status { get; set; } = string.Empty;


        // =====================================================
        // Hold
        // =====================================================
        public DateTime HoldExpiresAt { get; set; }


        // =====================================================
        // Audit
        // =====================================================
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CancelledAt { get; set; }
    }
}
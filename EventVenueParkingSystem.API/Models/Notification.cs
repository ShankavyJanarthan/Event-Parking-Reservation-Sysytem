namespace EventParkingReservationSystem.API.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }

        // =====================================================
        // Customer
        // =====================================================
        public int CustomerId { get; set; }

        public Customer? Customer { get; set; }


        // =====================================================
        // Optional Booking Reference
        // =====================================================
        public int? BookingId { get; set; }

        public Booking? Booking { get; set; }


        // =====================================================
        // Notification Type
        // BookingConfirmed
        // BookingCancelled
        // BookingExpired
        // PaymentSuccessful
        // =====================================================
        public string Type { get; set; } = string.Empty;


        // =====================================================
        // Notification Content
        // =====================================================
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;


        // =====================================================
        // Read Status
        // =====================================================
        public bool IsRead { get; set; } = false;

        public DateTime? ReadAt { get; set; }


        // =====================================================
        // Audit
        // =====================================================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
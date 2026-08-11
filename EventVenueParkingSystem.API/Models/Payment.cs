namespace EventParkingReservationSystem.API.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }

        // =====================================================
        // Booking
        // One successful payment per booking
        // =====================================================
        public int BookingId { get; set; }

        public Booking? Booking { get; set; }


        // =====================================================
        // Payment Amount
        // Booking TotalAmount snapshot
        // =====================================================
        public decimal Amount { get; set; }


        // =====================================================
        // Payment Method
        // Card / Cash / Simulated
        // =====================================================
        public string PaymentMethod { get; set; } = "Simulated";


        // =====================================================
        // Payment Status
        // Pending / Successful / Failed
        // =====================================================
        public string Status { get; set; } = "Pending";


        // =====================================================
        // Transaction Reference
        // Example: PAY-2026-000001
        // =====================================================
        public string TransactionReference { get; set; } = string.Empty;


        // =====================================================
        // Payment Date
        // =====================================================
        public DateTime? PaidAt { get; set; }


        // =====================================================
        // Audit
        // =====================================================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
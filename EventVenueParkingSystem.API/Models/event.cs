namespace EventParkingReservationSystem.API.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // =====================================================
        // Venue
        // =====================================================
        public int VenueId { get; set; }

        public Venue? Venue { get; set; }


        // =====================================================
        // Event Category
        // =====================================================
        public int EventCategoryId { get; set; }

        public EventCategory? EventCategory { get; set; }


        // =====================================================
        // Event Date / Time
        // =====================================================
        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }


        // =====================================================
        // Capacity
        // Must not exceed Venue Capacity
        // =====================================================
        public int Capacity { get; set; }


        // =====================================================
        // Ticket Price
        // =====================================================
        public decimal TicketPrice { get; set; }


        // =====================================================
        // Status
        // Draft / Active / Cancelled / Completed
        // =====================================================
        public string Status { get; set; } = "Active";


        // =====================================================
        // Audit
        // =====================================================
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
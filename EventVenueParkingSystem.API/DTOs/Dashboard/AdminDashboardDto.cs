namespace EventParkingReservationSystem.API.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalCustomers { get; set; }

        public int ActiveCustomers { get; set; }

        public int DeactivatedCustomers { get; set; }

        public int TotalEvents { get; set; }

        public int UpcomingEvents { get; set; }

        public int TotalBookings { get; set; }

        public int PendingBookings { get; set; }

        public int ConfirmedBookings { get; set; }

        public int CancelledBookings { get; set; }

        public int ExpiredBookings { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}
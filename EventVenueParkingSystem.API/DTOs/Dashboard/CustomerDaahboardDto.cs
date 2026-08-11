namespace EventParkingReservationSystem.API.DTOs.Dashboard
{
    public class CustomerDashboardDto
    {
        public int CustomerId { get; set; }

        public int TotalBookings { get; set; }

        public int PendingBookings { get; set; }

        public int ConfirmedBookings { get; set; }

        public int CancelledBookings { get; set; }

        public int ExpiredBookings { get; set; }

        public int UpcomingBookings { get; set; }

        public decimal TotalSpent { get; set; }
    }
}
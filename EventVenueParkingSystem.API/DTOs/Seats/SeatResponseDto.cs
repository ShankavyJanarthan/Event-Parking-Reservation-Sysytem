namespace EventParkingReservationSystem.API.DTOs.Seats
{
    public class SeatResponseDto
    {
        public int SeatId { get; set; }

        public int EventId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string RowLabel { get; set; } = string.Empty;

        public int SeatNumber { get; set; }

        public string SeatLabel { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
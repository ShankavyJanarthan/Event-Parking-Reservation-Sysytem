using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int SeatId { get; set; }

        // Optional
        // Maximum one parking slot per booking
        public int? ParkingSlotId { get; set; }
    }
}
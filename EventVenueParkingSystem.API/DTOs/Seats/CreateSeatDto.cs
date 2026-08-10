using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.Seats
{
    public class CreateSeatDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(20)]
        public string RowLabel { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int SeatNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string SeatLabel { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
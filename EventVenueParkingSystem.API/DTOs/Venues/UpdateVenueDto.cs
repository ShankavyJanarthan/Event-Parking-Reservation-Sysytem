using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.Venues
{
    public class UpdateVenueDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        public bool IsAvailable { get; set; }
    }
}
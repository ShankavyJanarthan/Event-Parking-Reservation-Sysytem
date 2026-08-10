using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.Events
{
    public class CreateEventDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int VenueId { get; set; }

        [Required]
        public int EventCategoryId { get; set; }

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TicketPrice { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Active";
    }
}
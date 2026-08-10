using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.ParkingSlots
{
    public class CreateParkingSlotDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        [MaxLength(50)]
        public string SlotNumber { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
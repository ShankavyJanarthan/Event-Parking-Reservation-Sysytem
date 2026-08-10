using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.ParkingSlots
{
    public class UpdateParkingSlotDto
    {
        [Required]
        [MaxLength(50)]
        public string SlotNumber { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Fee { get; set; }

        public bool IsActive { get; set; }
    }
}
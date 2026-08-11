using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.Payments
{
    public class ProcessPaymentDto
    {
        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = "Simulated";
    }
}
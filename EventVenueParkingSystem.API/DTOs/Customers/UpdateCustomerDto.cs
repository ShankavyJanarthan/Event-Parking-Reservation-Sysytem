using System.ComponentModel.DataAnnotations;

namespace EventParkingReservationSystem.API.DTOs.Customers
{
    public class UpdateCustomerDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(30)]
        public string Phone { get; set; } = string.Empty;
    }
}
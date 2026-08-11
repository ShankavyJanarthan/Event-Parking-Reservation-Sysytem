namespace EventParkingReservationSystem.API.DTOs.Customers
{
    public class CustomerResponseDto
    {
        public int CustomerId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public bool EmailVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
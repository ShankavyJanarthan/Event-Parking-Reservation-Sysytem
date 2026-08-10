namespace EventParkingReservationSystem.API.DTOs.EventCategories
{
    public class EventCategoryResponseDto
    {
        public int EventCategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
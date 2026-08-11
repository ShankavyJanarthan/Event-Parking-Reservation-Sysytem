namespace EventVenueParkingSystem.API.Models
{
    public class Venue
    {
    }
}
namespace EventParkingReservationSystem.API.Models
{
    public class Venue
    {
        public int VenueId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
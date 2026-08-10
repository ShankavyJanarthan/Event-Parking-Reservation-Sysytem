using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventVenueParkingSystem.API.Repositories.Implementations
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Event By Id
        // =====================================================
        public async Task<Event?> GetByIdAsync(int eventId)
        {
            return await _context.Events
                .Include(x => x.Venue)
                .Include(x => x.EventCategory)
                .FirstOrDefaultAsync(
                    x => x.EventId == eventId);
        }

        // =====================================================
        // Get / Search / Filter Events
        // =====================================================
        public async Task<List<Event>> GetAllAsync(
            string? search,
            int? venueId,
            int? eventCategoryId)
        {
            var query = _context.Events
                .Include(x => x.Venue)
                .Include(x => x.EventCategory)
                .AsQueryable();

            // Search by event name
            if (!string.IsNullOrWhiteSpace(search))
            {
                var value =
                    search.Trim().ToLowerInvariant();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(value));
            }

            // Filter by Venue
            if (venueId.HasValue)
            {
                query = query.Where(
                    x => x.VenueId == venueId.Value);
            }

            // Filter by Category
            if (eventCategoryId.HasValue)
            {
                query = query.Where(
                    x => x.EventCategoryId ==
                         eventCategoryId.Value);
            }

            return await query
                .OrderBy(x => x.StartDateTime)
                .ToListAsync();
        }

        // =====================================================
        // Check Event Time Overlap
        // =====================================================
        public async Task<bool> HasOverlappingEventAsync(
            int venueId,
            DateTime startDateTime,
            DateTime endDateTime,
            int? excludeEventId = null)
        {
            var query = _context.Events
                .Where(x =>
                    x.VenueId == venueId &&
                    x.Status != "Cancelled");

            // Update scenario:
            // Ignore the event currently being edited
            if (excludeEventId.HasValue)
            {
                query = query.Where(
                    x => x.EventId != excludeEventId.Value);
            }

            return await query.AnyAsync(x =>
                startDateTime < x.EndDateTime &&
                endDateTime > x.StartDateTime);
        }

        // =====================================================
        // Add
        // =====================================================
        public async Task AddAsync(Event eventItem)
        {
            await _context.Events.AddAsync(eventItem);
        }

        // =====================================================
        // Update
        // =====================================================
        public Task UpdateAsync(Event eventItem)
        {
            _context.Events.Update(eventItem);

            return Task.CompletedTask;
        }

        // =====================================================
        // Delete
        // =====================================================
        public Task DeleteAsync(Event eventItem)
        {
            _context.Events.Remove(eventItem);

            return Task.CompletedTask;
        }

        // =====================================================
        // Save
        // =====================================================
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
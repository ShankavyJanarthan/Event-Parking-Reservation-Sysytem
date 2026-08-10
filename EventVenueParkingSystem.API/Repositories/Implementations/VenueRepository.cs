using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class VenueRepository : IVenueRepository
    {
        private readonly ApplicationDbContext _context;

        public VenueRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Venue By Id
        // =====================================================
        public async Task<Venue?> GetByIdAsync(int venueId)
        {
            return await _context.Venues
                .FirstOrDefaultAsync(
                    x => x.VenueId == venueId);
        }

        // =====================================================
        // Get All Venues
        // =====================================================
        public async Task<List<Venue>> GetAllAsync()
        {
            return await _context.Venues
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =====================================================
        // Check Upcoming Events
        // Venue cannot be deleted if upcoming events exist
        // =====================================================
        public async Task<bool> HasUpcomingEventsAsync(
            int venueId)
        {
            return await _context.Events
                .AnyAsync(x =>
                    x.VenueId == venueId &&
                    x.StartDateTime > DateTime.UtcNow &&
                    x.Status != "Cancelled");
        }

        // =====================================================
        // Add
        // =====================================================
        public async Task AddAsync(Venue venue)
        {
            await _context.Venues.AddAsync(venue);
        }

        // =====================================================
        // Update
        // =====================================================
        public Task UpdateAsync(Venue venue)
        {
            _context.Venues.Update(venue);

            return Task.CompletedTask;
        }

        // =====================================================
        // Delete
        // =====================================================
        public Task DeleteAsync(Venue venue)
        {
            _context.Venues.Remove(venue);

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
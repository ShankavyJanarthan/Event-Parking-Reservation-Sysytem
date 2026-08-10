using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class EventCategoryRepository : IEventCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EventCategoryRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Category By Id
        // =====================================================
        public async Task<EventCategory?> GetByIdAsync(
            int eventCategoryId)
        {
            return await _context.EventCategories
                .FirstOrDefaultAsync(
                    x => x.EventCategoryId == eventCategoryId);
        }

        // =====================================================
        // Get Category By Name
        // =====================================================
        public async Task<EventCategory?> GetByNameAsync(
            string name)
        {
            var normalizedName =
                name.Trim().ToLowerInvariant();

            return await _context.EventCategories
                .FirstOrDefaultAsync(
                    x => x.Name.ToLower() == normalizedName);
        }

        // =====================================================
        // Get All Categories
        // =====================================================
        public async Task<List<EventCategory>> GetAllAsync()
        {
            return await _context.EventCategories
                .OrderBy(x => x.Name)
                .ToListAsync();
        }

        // =====================================================
        // Check Category Is Assigned To Events
        // =====================================================
        public async Task<bool> IsAssignedToEventsAsync(
            int eventCategoryId)
        {
            return await _context.Events
                .AnyAsync(
                    x => x.EventCategoryId == eventCategoryId);
        }

        // =====================================================
        // Add
        // =====================================================
        public async Task AddAsync(
            EventCategory eventCategory)
        {
            await _context.EventCategories
                .AddAsync(eventCategory);
        }

        // =====================================================
        // Update
        // =====================================================
        public Task UpdateAsync(
            EventCategory eventCategory)
        {
            _context.EventCategories.Update(eventCategory);

            return Task.CompletedTask;
        }

        // =====================================================
        // Delete
        // =====================================================
        public Task DeleteAsync(
            EventCategory eventCategory)
        {
            _context.EventCategories.Remove(eventCategory);

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
using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class ParkingSlotRepository : IParkingSlotRepository
    {
        private readonly ApplicationDbContext _context;

        public ParkingSlotRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Parking Slot By Id
        // =====================================================
        public async Task<ParkingSlot?> GetByIdAsync(
            int parkingSlotId)
        {
            return await _context.ParkingSlots
                .Include(x => x.Event)
                .FirstOrDefaultAsync(
                    x => x.ParkingSlotId == parkingSlotId);
        }

        // =====================================================
        // Get Parking Slots By Event
        // =====================================================
        public async Task<List<ParkingSlot>> GetByEventIdAsync(
            int eventId)
        {
            return await _context.ParkingSlots
                .Include(x => x.Event)
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.SlotNumber)
                .ToListAsync();
        }

        // =====================================================
        // Duplicate Parking Slot Validation
        // =====================================================
        public async Task<bool> SlotExistsAsync(
            int eventId,
            string slotNumber,
            int? excludeParkingSlotId = null)
        {
            var normalizedSlotNumber =
                slotNumber.Trim().ToLowerInvariant();

            var query =
                _context.ParkingSlots
                    .Where(x => x.EventId == eventId);

            // Update scenario:
            // Ignore current parking slot
            if (excludeParkingSlotId.HasValue)
            {
                query = query.Where(
                    x => x.ParkingSlotId !=
                         excludeParkingSlotId.Value);
            }

            return await query.AnyAsync(
                x => x.SlotNumber.ToLower() ==
                     normalizedSlotNumber);
        }

        // =====================================================
        // Add
        // =====================================================
        public async Task AddAsync(
            ParkingSlot parkingSlot)
        {
            await _context.ParkingSlots.AddAsync(
                parkingSlot);
        }

        // =====================================================
        // Update
        // =====================================================
        public Task UpdateAsync(
            ParkingSlot parkingSlot)
        {
            _context.ParkingSlots.Update(
                parkingSlot);

            return Task.CompletedTask;
        }

        // =====================================================
        // Delete
        // =====================================================
        public Task DeleteAsync(
            ParkingSlot parkingSlot)
        {
            _context.ParkingSlots.Remove(
                parkingSlot);

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
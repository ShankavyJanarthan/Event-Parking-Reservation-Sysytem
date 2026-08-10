using EventParkingReservationSystem.API.Data;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventParkingReservationSystem.API.Repositories.Implementations
{
    public class SeatRepository : ISeatRepository
    {
        private readonly ApplicationDbContext _context;

        public SeatRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // Get Seat By Id
        // =====================================================
        public async Task<Seat?> GetByIdAsync(int seatId)
        {
            return await _context.Seats
                .Include(x => x.Event)
                .FirstOrDefaultAsync(
                    x => x.SeatId == seatId);
        }

        // =====================================================
        // Get Seats By Event
        // =====================================================
        public async Task<List<Seat>> GetByEventIdAsync(
            int eventId)
        {
            return await _context.Seats
                .Include(x => x.Event)
                .Where(x => x.EventId == eventId)
                .OrderBy(x => x.RowLabel)
                .ThenBy(x => x.SeatNumber)
                .ToListAsync();
        }

        // =====================================================
        // Get Seat Count For Event
        // =====================================================
        public async Task<int> GetSeatCountByEventAsync(
            int eventId)
        {
            return await _context.Seats
                .CountAsync(
                    x => x.EventId == eventId);
        }

        // =====================================================
        // Duplicate Seat Validation
        // =====================================================
        public async Task<bool> SeatExistsAsync(
            int eventId,
            string rowLabel,
            int seatNumber,
            string seatLabel,
            int? excludeSeatId = null)
        {
            var normalizedRow =
                rowLabel.Trim().ToLowerInvariant();

            var normalizedLabel =
                seatLabel.Trim().ToLowerInvariant();

            var query =
                _context.Seats.Where(
                    x => x.EventId == eventId);

            // Update scenario:
            // Ignore current seat
            if (excludeSeatId.HasValue)
            {
                query = query.Where(
                    x => x.SeatId != excludeSeatId.Value);
            }

            return await query.AnyAsync(x =>
                (
                    x.RowLabel.ToLower() == normalizedRow &&
                    x.SeatNumber == seatNumber
                )
                ||
                x.SeatLabel.ToLower() == normalizedLabel);
        }

        // =====================================================
        // Add
        // =====================================================
        public async Task AddAsync(Seat seat)
        {
            await _context.Seats.AddAsync(seat);
        }

        // =====================================================
        // Update
        // =====================================================
        public Task UpdateAsync(Seat seat)
        {
            _context.Seats.Update(seat);

            return Task.CompletedTask;
        }

        // =====================================================
        // Delete
        // =====================================================
        public Task DeleteAsync(Seat seat)
        {
            _context.Seats.Remove(seat);

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
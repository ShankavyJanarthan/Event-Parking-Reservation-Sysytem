using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.BackgroundServices
{
    public class BookingExpiryBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingExpiryBackgroundService> _logger;

        public BookingExpiryBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<BookingExpiryBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Booking expiry background service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ExpireBookingsAsync(
                        stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "An error occurred while expiring bookings.");
                }

                try
                {
                    await Task.Delay(
                        TimeSpan.FromMinutes(1),
                        stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task ExpireBookingsAsync(
            CancellationToken stoppingToken)
        {
            using var scope =
                _scopeFactory.CreateScope();

            var bookingRepository =
                scope.ServiceProvider
                    .GetRequiredService<IBookingRepository>();

            var notificationService =
                scope.ServiceProvider
                    .GetRequiredService<INotificationService>();

            var currentTime =
                DateTime.UtcNow;

            var expiredBookings =
                await bookingRepository
                    .GetExpiredPendingBookingsAsync(
                        currentTime);

            if (expiredBookings.Count == 0)
            {
                return;
            }

            var processedBookings =
                new List<Models.Booking>();

            foreach (var booking in expiredBookings)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                if (!string.Equals(
                        booking.Status,
                        "Pending",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (booking.HoldExpiresAt > currentTime)
                {
                    continue;
                }

                booking.Status = "Expired";
                booking.UpdatedAt = currentTime;

                await bookingRepository
                    .UpdateAsync(booking);

                processedBookings.Add(booking);
            }

            if (processedBookings.Count == 0)
            {
                return;
            }

            await bookingRepository
                .SaveChangesAsync();

            foreach (var booking in processedBookings)
            {
                try
                {
                    await notificationService.CreateAsync(
                        booking.CustomerId,
                        booking.BookingId,
                        "BookingExpired",
                        "Booking Expired",
                        $"Your booking {booking.BookingNumber} expired because payment was not completed within the booking hold period.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Failed to create expiry notification for booking {BookingId}.",
                        booking.BookingId);
                }
            }

            _logger.LogInformation(
                "{Count} pending booking(s) expired.",
                processedBookings.Count);
        }
    }
}
using EventParkingReservationSystem.API.DTOs.Customers;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IBookingRepository _bookingRepository;

        public CustomerService(
            ICustomerRepository customerRepository,
            IBookingRepository bookingRepository)
        {
            _customerRepository = customerRepository;
            _bookingRepository = bookingRepository;
        }

        // =====================================================
        // Get Customer
        // =====================================================
        public async Task<CustomerResponseDto> GetByIdAsync(
            int customerId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer was not found.");
            }

            return MapToResponse(customer);
        }

        // =====================================================
        // Update Customer Profile
        // =====================================================
        public async Task<CustomerResponseDto> UpdateAsync(
            int customerId,
            UpdateCustomerDto dto)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer was not found.");
            }

            if (customer.Status.Equals(
                    "Deactivated",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "A deactivated customer account cannot be updated.");
            }

            customer.FirstName =
                dto.FirstName.Trim();

            customer.LastName =
                dto.LastName.Trim();

            customer.Phone =
                dto.Phone.Trim();

            customer.UpdatedAt =
                DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);

            await _customerRepository.SaveChangesAsync();

            return MapToResponse(customer);
        }

        // =====================================================
        // Search Customers - Admin use
        // =====================================================
        public async Task<List<CustomerResponseDto>> SearchAsync(
            string? search)
        {
            var customers =
                await _customerRepository.SearchAsync(search);

            return customers
                .Select(MapToResponse)
                .ToList();
        }

        // =====================================================
        // Deactivate Customer - Admin use
        //
        // Customer cannot be deactivated when they have
        // an active future Pending / Confirmed booking.
        // =====================================================
        public async Task<string> DeactivateAsync(
            int customerId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(
                    customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer was not found.");
            }

            if (customer.Role.Equals(
                    "Admin",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Admin accounts cannot be deactivated through this operation.");
            }

            if (customer.Status.Equals(
                    "Deactivated",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Customer account is already deactivated.");
            }

            var hasActiveFutureBooking =
                await _bookingRepository
                    .HasActiveFutureBookingsForCustomerAsync(
                        customerId);

            if (hasActiveFutureBooking)
            {
                throw new InvalidOperationException(
                    "Customer account cannot be deactivated because there is an active future booking.");
            }

            customer.Status =
                "Deactivated";

            customer.UpdatedAt =
                DateTime.UtcNow;

            await _customerRepository.UpdateAsync(
                customer);

            await _customerRepository.SaveChangesAsync();

            return
                "Customer account deactivated successfully.";
        }

        // =====================================================
        // Reactivate Customer - Admin use
        // =====================================================
        public async Task<string> ReactivateAsync(
            int customerId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                throw new KeyNotFoundException(
                    "Customer was not found.");
            }

            if (customer.Status.Equals(
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Customer account is already active.");
            }

            customer.Status =
                "Active";

            customer.UpdatedAt =
                DateTime.UtcNow;

            await _customerRepository.UpdateAsync(
                customer);

            await _customerRepository.SaveChangesAsync();

            return
                "Customer account reactivated successfully.";
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static CustomerResponseDto MapToResponse(
            Customer customer)
        {
            return new CustomerResponseDto
            {
                CustomerId =
                    customer.CustomerId,

                FirstName =
                    customer.FirstName,

                LastName =
                    customer.LastName,

                FullName =
                    $"{customer.FirstName} {customer.LastName}",

                Email =
                    customer.Email,

                Phone =
                    customer.Phone,

                Role =
                    customer.Role,

                Status =
                    customer.Status,

                EmailVerified =
                    customer.EmailVerified,

                CreatedAt =
                    customer.CreatedAt,

                UpdatedAt =
                    customer.UpdatedAt
            };
        }
    }
}
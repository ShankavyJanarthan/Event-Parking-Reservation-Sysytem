using EventParkingReservationSystem.API.DTOs.Customers;
using EventParkingReservationSystem.API.Models;
using EventParkingReservationSystem.API.Repositories.Interfaces;
using EventParkingReservationSystem.API.Services.Interfaces;

namespace EventParkingReservationSystem.API.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(
            ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
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

            customer.Status = "Active";

            customer.UpdatedAt =
                DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);

            await _customerRepository.SaveChangesAsync();

            return "Customer account reactivated successfully.";
        }

        // =====================================================
        // Entity -> Response DTO
        // =====================================================
        private static CustomerResponseDto MapToResponse(
            Customer customer)
        {
            return new CustomerResponseDto
            {
                CustomerId = customer.CustomerId,

                FirstName = customer.FirstName,

                LastName = customer.LastName,

                FullName =
                    $"{customer.FirstName} {customer.LastName}",

                Email = customer.Email,

                Phone = customer.Phone,

                Role = customer.Role,

                Status = customer.Status,

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
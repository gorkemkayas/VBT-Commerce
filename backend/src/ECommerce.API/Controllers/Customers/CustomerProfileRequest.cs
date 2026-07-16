namespace ECommerce.API.Controllers.Customers;

public record CreateCustomerProfileRequest(string? PhoneNumber, DateOnly? DateOfBirth);

public record UpdateCustomerProfileRequest(string? PhoneNumber, DateOnly? DateOfBirth);

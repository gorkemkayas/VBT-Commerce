namespace ECommerce.API.Controllers.Customers;

public record CreateGuestCustomerRequest(string FirstName, string LastName, string Email, string PhoneNumber);

namespace ECommerce.API.Controllers.Customers.Requests;

public record CreateGuestCustomerRequest(string FirstName, string LastName, string Email, string PhoneNumber);

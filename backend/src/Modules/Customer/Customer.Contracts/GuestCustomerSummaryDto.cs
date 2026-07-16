namespace Customer.Contracts;

public record GuestCustomerSummaryDto(Guid Id, string FirstName, string LastName, string Email, string PhoneNumber);

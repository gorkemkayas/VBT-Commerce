using BuildingBlocks.Application.Messaging;

namespace Customer.Application.Commands.GuestCustomers.CreateGuestCustomer;

/// <summary>
/// Intentionally unauthenticated — a guest, by definition, has not logged in. Used by the future
/// Cart/Order guest checkout flow.
/// </summary>
public record CreateGuestCustomerCommand(string FirstName, string LastName, string Email, string PhoneNumber) : ICommand<Guid>;

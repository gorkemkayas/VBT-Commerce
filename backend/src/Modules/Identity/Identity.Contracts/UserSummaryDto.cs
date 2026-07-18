namespace Identity.Contracts;

public record UserSummaryDto(Guid Id, string Email, string FirstName, string LastName);

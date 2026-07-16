namespace Customer.Contracts;

public record CustomerSummaryDto(Guid Id, Guid UserId, string? PhoneNumber);

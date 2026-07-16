namespace Inventory.Contracts;

public record ReserveStockResult(bool IsSuccess, Guid ReferenceId, string? ErrorMessage);

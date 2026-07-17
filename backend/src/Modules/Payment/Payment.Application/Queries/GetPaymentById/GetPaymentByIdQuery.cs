using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Payment.Application.Common;

namespace Payment.Application.Queries.GetPaymentById;

public record GetPaymentByIdQuery(Guid PaymentId) : IQuery<PaymentDto>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}

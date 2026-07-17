using BuildingBlocks.Application.Messaging;
using BuildingBlocks.Application.Security;
using Payment.Application.Common;
using Payment.Domain.Enums;

namespace Payment.Application.Queries.GetPaymentsList;

public record GetPaymentsListQuery(PaymentStatus? Status = null, int PageNumber = 1, int PageSize = 20)
    : IQuery<PagedResult<PaymentDto>>, IRequireRole
{
    public string[] AllowedRoles => ["Admin"];
}

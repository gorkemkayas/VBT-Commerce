using MediatR;
using Payment.Application.Abstractions;
using Payment.Application.Common;
using Payment.Domain.Exceptions;

namespace Payment.Application.Queries.GetPaymentById;

public class GetPaymentByIdQueryHandler(IPaymentDbContext dbContext) : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
{
    public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await dbContext.Payments.FindAsync([request.PaymentId], cancellationToken)
            ?? throw new PaymentNotFoundException(request.PaymentId);

        return PaymentMapper.ToDto(payment);
    }
}

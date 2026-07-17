using BuildingBlocks.Application.Messaging;
using MediatR;
using Pricing.Application.Abstractions;

namespace Pricing.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IPricingDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is not ICommand<TResponse>)
            return await next();

        await using var transaction = await dbContext.BeginTransactionAsync(cancellationToken);
        try
        {
            var response = await next();
            await transaction.CommitAsync(cancellationToken);
            return response;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}

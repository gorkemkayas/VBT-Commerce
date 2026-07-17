using BuildingBlocks.Application.Messaging;
using MediatR;
using Payment.Application.Abstractions;

namespace Payment.Application.Behaviors;

public class TransactionBehavior<TRequest, TResponse>(IPaymentDbContext dbContext)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Every module's TransactionBehavior is a global AddOpenBehavior, so it sees every command in
        // the whole system, not just its own — this check keeps it a no-op for other modules' commands.
        // Necessary because Order calls Payment's commands in-process via ISender: without this,
        // a nested call would try to re-open a transaction on a DbContext that's already mid-transaction.
        if (request is not ICommand<TResponse> || typeof(TRequest).Assembly != typeof(IPaymentDbContext).Assembly)
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

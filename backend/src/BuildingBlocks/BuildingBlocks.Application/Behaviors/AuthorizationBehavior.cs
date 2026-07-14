using BuildingBlocks.Application.Exceptions;
using BuildingBlocks.Application.Security;
using MediatR;

namespace BuildingBlocks.Application.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(ICurrentUserService currentUserService)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (request is IRequireRole requireRole)
        {
            if (!currentUserService.IsAuthenticated)
                throw new UnauthorizedException("Authentication is required to perform this action.");

            if (!requireRole.AllowedRoles.Contains(currentUserService.Role))
                throw new ForbiddenException("You are not authorized to perform this action.");
        }

        return next();
    }
}

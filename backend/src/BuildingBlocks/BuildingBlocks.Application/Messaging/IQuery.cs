using MediatR;

namespace BuildingBlocks.Application.Messaging;

public interface IQuery<TResponse> : IRequest<TResponse>
{
}

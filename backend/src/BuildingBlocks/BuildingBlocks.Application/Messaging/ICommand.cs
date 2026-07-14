using MediatR;

namespace BuildingBlocks.Application.Messaging;

public interface ICommand<TResponse> : IRequest<TResponse>
{
}

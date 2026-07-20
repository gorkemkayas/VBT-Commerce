using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Identity.Application.Commands.ForgotPassword;

public record ForgotPasswordCommand(string Email) : ICommand<Unit>;

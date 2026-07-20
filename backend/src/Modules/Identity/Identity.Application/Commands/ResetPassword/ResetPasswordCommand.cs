using BuildingBlocks.Application.Messaging;
using MediatR;

namespace Identity.Application.Commands.ResetPassword;

public record ResetPasswordCommand(string Token, string NewPassword) : ICommand<Unit>;

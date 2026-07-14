using BuildingBlocks.Application.Messaging;
using Identity.Application.Common;
using Identity.Domain.Enums;

namespace Identity.Application.Commands.RefreshTokens;

public record RefreshTokenCommand(string RefreshToken, ClientPlatform Platform) : ICommand<AuthResult>;

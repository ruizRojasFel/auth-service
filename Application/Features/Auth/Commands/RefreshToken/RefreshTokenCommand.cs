using Auth.API.Application.DTOs;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string Token
) : IRequest<AuthTokensDto>;
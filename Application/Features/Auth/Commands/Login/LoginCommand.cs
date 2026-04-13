using Auth.API.Application.DTOs;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthTokensDto>;
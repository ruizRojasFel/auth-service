using Auth.API.Application.DTOs;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.Register;

public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<AuthTokensDto>;
using Auth.API.Application.Common.Interfaces;
using Auth.API.Application.DTOs;
using Auth.API.Domain.Entities;
using Auth.API.Domain.Interfaces;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthTokensDto>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IJwtService _jwt;
    private readonly IUnitOfWork _uow;

    public RefreshTokenCommandHandler(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IJwtService jwt,
        IUnitOfWork uow)
    {
        _users         = users;
        _refreshTokens = refreshTokens;
        _jwt           = jwt;
        _uow           = uow;
    }

    public async Task<AuthTokensDto> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var existing = await _refreshTokens.GetByTokenAsync(cmd.Token, ct)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (!existing.IsActive)
            throw new UnauthorizedAccessException("Refresh token is expired or revoked.");

        var user = await _users.GetByIdAsync(existing.UserId, ct)
            ?? throw new UnauthorizedAccessException("User not found.");

        existing.Revoke();
        await _refreshTokens.UpdateAsync(existing, ct);

        var accessToken   = _jwt.GenerateAccessToken(user);
        var refreshRaw    = _jwt.GenerateRefreshToken();
        var refreshEntity = Domain.Entities.RefreshToken.Create(user.Id, refreshRaw);

        await _refreshTokens.AddAsync(refreshEntity, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthTokensDto(accessToken, refreshRaw, DateTime.UtcNow.AddMinutes(60));
    }
}
using Auth.API.Application.Common.Interfaces;
using Auth.API.Application.DTOs;
using Auth.API.Domain.Entities;
using Auth.API.Domain.Interfaces;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthTokensDto>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;
    private readonly IUnitOfWork _uow;

    public LoginCommandHandler(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPasswordHasher hasher,
        IJwtService jwt,
        IUnitOfWork uow)
    {
        _users         = users;
        _refreshTokens = refreshTokens;
        _hasher        = hasher;
        _jwt           = jwt;
        _uow           = uow;
    }

    public async Task<AuthTokensDto> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(cmd.Email, ct)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Account is disabled.");

        if (!_hasher.Verify(cmd.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        user.RecordLogin();
        await _users.UpdateAsync(user, ct);

        var accessToken   = _jwt.GenerateAccessToken(user);
        var refreshRaw    = _jwt.GenerateRefreshToken();
        var refreshEntity = Domain.Entities.RefreshToken.Create(user.Id, refreshRaw);

        await _refreshTokens.AddAsync(refreshEntity, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthTokensDto(accessToken, refreshRaw, DateTime.UtcNow.AddMinutes(60));
    }
}
using Auth.API.Application.Common.Interfaces;
using Auth.API.Application.DTOs;
using Auth.API.Domain.Entities;
using Auth.API.Domain.Interfaces;
using MediatR;

namespace Auth.API.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthTokensDto>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtService _jwt;
    private readonly IUnitOfWork _uow;

    public RegisterCommandHandler(
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

    public async Task<AuthTokensDto> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        if (await _users.ExistsByEmailAsync(cmd.Email, ct))
            throw new InvalidOperationException("Email already registered.");

        var hash          = _hasher.Hash(cmd.Password);
        var user          = User.Create(cmd.Email, hash, cmd.FirstName, cmd.LastName);
        var accessToken   = _jwt.GenerateAccessToken(user);
        var refreshRaw    = _jwt.GenerateRefreshToken();
        var refreshEntity = Domain.Entities.RefreshToken.Create(user.Id, refreshRaw);

        await _users.AddAsync(user, ct);
        await _refreshTokens.AddAsync(refreshEntity, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthTokensDto(accessToken, refreshRaw, DateTime.UtcNow.AddMinutes(60));
    }
}
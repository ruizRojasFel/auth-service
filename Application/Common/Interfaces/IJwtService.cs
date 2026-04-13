using Auth.API.Domain.Entities;

namespace Auth.API.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}
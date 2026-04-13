namespace Auth.API.Application.DTOs;

public record AuthTokensDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);
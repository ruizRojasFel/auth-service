namespace Auth.API.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Token { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive  => !IsRevoked && !IsExpired;

    protected RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, int expiryDays = 7)
    {
        return new RefreshToken
        {
            UserId    = userId,
            Token     = token,
            ExpiresAt = DateTime.UtcNow.AddDays(expiryDays)
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
    }
}
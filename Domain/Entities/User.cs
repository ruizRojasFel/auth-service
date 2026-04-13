namespace Auth.API.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; private set; }

    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    protected User() { }

    public static User Create(string email, string passwordHash, string firstName, string lastName)
    {
        return new User
        {
            Email        = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName    = firstName,
            LastName     = lastName
        };
    }

    public void RecordLogin() => LastLoginAt = DateTime.UtcNow;
    public void Deactivate()  => IsActive = false;
}
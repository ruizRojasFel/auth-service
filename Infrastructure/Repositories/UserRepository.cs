using Auth.API.Domain.Entities;
using Auth.API.Domain.Interfaces;
using Auth.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _db;
    public UserRepository(AuthDbContext db) => _db = db;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task AddAsync(User user, CancellationToken ct = default) =>
        await _db.Users.AddAsync(user, ct);

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }
}
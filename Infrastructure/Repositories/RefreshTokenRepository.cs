using Auth.API.Domain.Entities;
using Auth.API.Domain.Interfaces;
using Auth.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Auth.API.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthDbContext _db;
    public RefreshTokenRepository(AuthDbContext db) => _db = db;

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default) =>
        _db.RefreshTokens.Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct = default) =>
        await _db.RefreshTokens.AddAsync(token, ct);

    public Task UpdateAsync(RefreshToken token, CancellationToken ct = default)
    {
        _db.RefreshTokens.Update(token);
        return Task.CompletedTask;
    }
}
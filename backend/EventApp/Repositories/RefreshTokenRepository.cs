using EventApp.Data;
using EventApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Repositories;

public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokens.AddAsync(token, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => dbContext.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.RevokedAt == null && x.ExpiresAt > DateTime.UtcNow, cancellationToken);

    public async Task RevokeByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        var token = await dbContext.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.RevokedAt == null, cancellationToken);
        if (token is null)
        {
            return;
        }

        token.RevokedAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await dbContext.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        if (tokens.Count == 0)
        {
            return;
        }

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

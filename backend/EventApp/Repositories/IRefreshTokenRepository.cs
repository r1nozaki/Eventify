using EventApp.Models;

namespace EventApp.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetActiveByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task RevokeByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

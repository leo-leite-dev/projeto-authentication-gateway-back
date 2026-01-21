using AuthService.Domain.Entities;

namespace AuthService.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetActiveByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

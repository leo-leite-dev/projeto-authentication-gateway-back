using AuthService.Domain.Entities;

namespace AuthService.Application.Abstractions.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(
        string token,
        CancellationToken cancellationToken = default
    );
    Task RevokeAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
}

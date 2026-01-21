using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;

namespace AuthService.Application.UseCases.Auth.Logout;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;

    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository, ICurrentUser currentUser)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await _refreshTokenRepository.RevokeAllByUserAsync(_currentUser.UserId, cancellationToken);
    }
}

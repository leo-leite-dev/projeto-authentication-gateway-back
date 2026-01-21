using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Time;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.RefreshTokens;

public sealed class RefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IDateTimeProvider _dateTime;

    public RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        ICurrentUser currentUser,
        IDateTimeProvider dateTime
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<RefreshTokenResult> ExecuteAsync(
        CancellationToken cancellationToken = default
    )
    {
        var refreshToken =
            await _refreshTokenRepository.GetActiveByUserAsync(
                _currentUser.UserId,
                cancellationToken
            ) ?? throw new UserException("Sessão inválida ou inexistente.");

        if (refreshToken.IsRevoked || refreshToken.IsExpired())
            throw new UserException("Sessão expirada ou revogada.");

        return new RefreshTokenResult(_dateTime.UtcNow);
    }
}

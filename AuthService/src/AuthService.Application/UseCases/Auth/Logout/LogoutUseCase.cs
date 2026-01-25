using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.Logout;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutUseCase(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task ExecuteAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var tokenEntity =
            await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken)
            ?? throw new UserException("Sessão inválida.");

        await _refreshTokenRepository.RevokeAsync(tokenEntity, cancellationToken);
    }
}

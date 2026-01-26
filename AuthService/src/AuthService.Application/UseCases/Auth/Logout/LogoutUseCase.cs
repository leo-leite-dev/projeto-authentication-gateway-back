using AuthService.Application.Abstractions.Repositories;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.Logout;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;

    public LogoutUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
    }

    public async Task ExecuteAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default
    )
    {
        var tokenEntity =
            await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken)
            ?? throw new UserException("Sessão inválida.");

        var user = tokenEntity.User ?? throw new UserException("Usuário não encontrado.");

        user.IncrementTokenVersion();

        await _refreshTokenRepository.RevokeAllByUserAsync(user.Id, cancellationToken);
        await _userRepository.UpdateAsync(user, cancellationToken);
    }
}

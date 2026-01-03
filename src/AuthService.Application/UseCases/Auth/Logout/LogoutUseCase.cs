using AuthService.Application.Abstractions.Repositories;

namespace AuthService.Application.UseCases.Auth.Logout;

public sealed class LogoutUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly LogoutCommandValidator _validator;

    public LogoutUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        LogoutCommandValidator validator
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _validator = validator;
    }

    public async Task ExecuteAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _validator.Validate(command);

        await _refreshTokenRepository.RevokeAllByUserAsync(command.UserId, cancellationToken);
    }
}

using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Time;
using AuthService.Domain.Enums;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.RefreshTokens;

public sealed class RefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTime;
    private readonly RefreshTokenCommandValidator _validator;

    public RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IDateTimeProvider dateTime,
        RefreshTokenCommandValidator validator
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _dateTime = dateTime;
        _validator = validator;
    }

    public async Task<RefreshTokenResult> ExecuteAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _validator.Validate(command);

        var refreshToken =
            await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken)
            ?? throw new UserException("Refresh token inválido.");

        if (refreshToken.IsRevoked || refreshToken.IsExpired())
            throw new UserException("Refresh token expirado ou revogado.");

        var accessToken = _tokenService.GenerateToken(refreshToken.User, TokenType.AccessToken);

        return new RefreshTokenResult(accessToken, _dateTime.UtcNow.AddMinutes(15));
    }
}

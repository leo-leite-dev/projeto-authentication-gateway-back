using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Security;
using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;

namespace AuthService.Application.UseCases.Auth.RefreshTokens;

public sealed class RefreshTokenUseCase
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _tokenService;
    private readonly IUserRepository _userRepository;

    public RefreshTokenUseCase(
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService tokenService,
        IUserRepository userRepository
    )
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _userRepository = userRepository;
    }

    public async Task<RefreshTokenResult> ExecuteAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken
    )
    {
        var tokenEntity =
            await _refreshTokenRepository.GetByTokenAsync(command.RefreshToken, cancellationToken)
            ?? throw new UserException("Sessão inválida ou expirada");

        var user =
            await _userRepository.GetByIdAsync(tokenEntity.UserId, cancellationToken)
            ?? throw new UserException("Usuário não encontrado");

        await _refreshTokenRepository.RevokeAsync(tokenEntity, cancellationToken);

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(7);

        await _refreshTokenRepository.AddAsync(
            new RefreshToken(user.Id, newRefreshToken, refreshTokenExpiresAt),
            cancellationToken
        );

        return new RefreshTokenResult(newAccessToken, newRefreshToken);
    }
}

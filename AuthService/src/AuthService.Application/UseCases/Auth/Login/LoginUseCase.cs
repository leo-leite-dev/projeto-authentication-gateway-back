using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Time;
using AuthService.Application.Security;
using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using AuthService.Domain.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace AuthService.Application.UseCases.Auth.Login;

public sealed class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTime;
    private readonly LoginCommandValidator _validator;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTime,
        LoginCommandValidator validator,
        IJwtTokenService jwtTokenService
    )
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _dateTime = dateTime;
        _validator = validator;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResult> ExecuteAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _validator.Validate(command);

        var user = await ResolveUserAsync(command.Login, cancellationToken);

        if (!user.CanLogin())
            throw new UserException("Usuário não pode autenticar.");

        if (!_passwordHasher.Verify(command.Password, user.PasswordHash))
            throw new UserException("Usuário ou senha inválidos.");

        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshTokenExpiresAt = _dateTime.UtcNow.AddDays(7);

        var refreshToken = new RefreshToken(
            userId: user.Id,
            token: refreshTokenValue,
            expiresAt: refreshTokenExpiresAt
        );

        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);

        return new LoginResult(
            user.Id,
            user.Username.Value,
            user.Email.Value,
            user.Status.ToString(),
            accessToken,
            refreshTokenValue
        );
    }

    private async Task<User> ResolveUserAsync(string login, CancellationToken cancellationToken)
    {
        if (Email.TryParse(login, out var email))
            return await _userRepository.GetByEmailAsync(email, cancellationToken)
                ?? throw new UserException("Usuário ou senha inválidos.");

        if (Username.TryParse(login, out var username))
            return await _userRepository.GetByUsernameAsync(username, cancellationToken)
                ?? throw new UserException("Usuário ou senha inválidos.");

        throw new UserException("Login inválido.");
    }
}

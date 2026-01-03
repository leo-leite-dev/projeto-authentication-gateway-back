using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Time;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using AuthService.Domain.Exceptions;
using AuthService.Domain.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace AuthService.Application.UseCases.Auth.Login;

public sealed class LoginUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTime;
    private readonly LoginCommandValidator _validator;

    public LoginUseCase(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTimeProvider dateTime,
        LoginCommandValidator validator
    )
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTime = dateTime;
        _validator = validator;
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

        var accessToken = _tokenService.GenerateToken(user, TokenType.AccessToken);
        var refreshTokenValue = _tokenService.GenerateToken(user, TokenType.RefreshToken);

        var refreshToken = new RefreshToken(
            Guid.NewGuid(),
            refreshTokenValue,
            _dateTime.UtcNow.AddDays(7),
            user.Id
        );

        user.AddRefreshToken(refreshToken);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return new LoginResult(accessToken, refreshTokenValue, _dateTime.UtcNow.AddMinutes(15));
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

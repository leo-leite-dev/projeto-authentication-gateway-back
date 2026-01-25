using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Time;
using AuthService.Domain.Entities;
using AuthService.Domain.Exceptions;
using AuthService.Domain.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace AuthService.Application.UseCases.Auth.Register;

public sealed class RegisterUserUseCase
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTime;
    private readonly RegisterUserCommandValidator _validator;

    public RegisterUserUseCase(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTime,
        RegisterUserCommandValidator validator
    )
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _dateTime = dateTime;
        _validator = validator;
    }

    public async Task<RegisterUserResult> ExecuteAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default
    )
    {
        _validator.Validate(command);

        var email = Email.Parse(command.Email);
        var username = Username.Parse(command.Username);

        if (await _userRepository.ExistsByEmailAsync(email, cancellationToken))
            throw new UserException("Email já está em uso.");

        if (await _userRepository.ExistsByUsernameAsync(username, cancellationToken))
            throw new UserException("Username já está em uso.");

        var passwordHash = new PasswordHash(_passwordHasher.Hash(command.Password));

        var user = new User(
            id: Guid.NewGuid(),
            username: username,
            email: email,
            passwordHash: passwordHash,
            createdAt: _dateTime.UtcNow
        );

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterUserResult(
            user.Id,
            user.Username.Value,
            user.Email.Value,
            user.Status.ToString()
        );
    }
}

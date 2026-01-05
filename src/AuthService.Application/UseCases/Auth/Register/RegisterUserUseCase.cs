using AuthService.Application.Abstractions.Repositories;
using AuthService.Application.Abstractions.Security;
using AuthService.Application.Abstractions.Time;
using AuthService.Application.Security;
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

        var password = new Password(command.Password);

        var passwordHash = new PasswordHash(_passwordHasher.Hash(password.Value));

        var user = new User(Guid.NewGuid(), username, email, passwordHash, _dateTime.UtcNow);

        await _userRepository.AddAsync(user, cancellationToken);

        return new RegisterUserResult(user.Id);
    }
}

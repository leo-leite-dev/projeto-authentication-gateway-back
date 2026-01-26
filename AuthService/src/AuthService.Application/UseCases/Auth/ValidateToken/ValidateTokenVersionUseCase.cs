using AuthService.Application.Abstractions.Repositories;

namespace AuthService.Application.UseCases.Auth.ValidateToken;

public sealed class ValidateTokenVersionUseCase
{
    private readonly IUserRepository _userRepository;

    public ValidateTokenVersionUseCase(IUserRepository userRepository) =>
        _userRepository = userRepository;

    public async Task<bool> ExecuteAsync(Guid userId, int tokenVersion)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user is not null && user.TokenVersion == tokenVersion;
    }
}

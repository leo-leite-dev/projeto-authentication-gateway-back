using AuthService.Domain.Enums;
using AuthService.Domain.ValueObjects;
using BaitaHora.Domain.Features.Common.ValueObjects;

namespace AuthService.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Username Username { get; private set; }
    public Email Email { get; private set; }
    public PasswordHash PasswordHash { get; private set; } = null!;
    public UserStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public int TokenVersion { get; private set; }

    private readonly List<RefreshToken> _refreshTokens = new();
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    protected User() { }

    public User(
        Guid id,
        Username username,
        Email email,
        PasswordHash passwordHash,
        DateTime createdAt
    )
    {
        Id = id;
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
        Status = UserStatus.Active;
        TokenVersion = 0;
    }

    public void Deactivate()
    {
        Status = UserStatus.Inactive;
    }

    public void Activate()
    {
        Status = UserStatus.Active;
    }

    public void Suspend()
    {
        Status = UserStatus.Suspended;
    }

    public bool CanLogin()
    {
        return Status == UserStatus.Active;
    }

    public void ChangePassword(PasswordHash newPasswordHash)
    {
        PasswordHash = newPasswordHash;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _refreshTokens.Add(refreshToken);
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens)
        {
            token.Revoke();
        }
    }

    public void IncrementTokenVersion()
    {
        TokenVersion++;
    }
}

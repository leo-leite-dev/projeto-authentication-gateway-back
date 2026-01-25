namespace AuthService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }

    public string Token { get; private set; } = null!;

    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected RefreshToken() { }

    public RefreshToken(Guid userId, string token, DateTime expiresAt)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;

    public bool IsActive() => !IsRevoked && !IsExpired();

    public void Revoke() => IsRevoked = true;
}

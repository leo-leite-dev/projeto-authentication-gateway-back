namespace AuthService.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected RefreshToken() { }

    public RefreshToken(Guid id, Guid userId, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;

    public void Revoke() => IsRevoked = true;
}

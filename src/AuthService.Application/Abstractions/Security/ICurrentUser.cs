namespace AuthService.Application.Abstractions.Security;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Username { get; }
    string Email { get; }
    string Status { get; }
}

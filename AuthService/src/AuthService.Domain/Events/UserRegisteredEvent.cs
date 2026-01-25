namespace AuthService.Domain.Events;

public sealed record UserRegisteredEvent(
    Guid UserId,
    string Username,
    string Email,
    DateTime OccurredAt
);

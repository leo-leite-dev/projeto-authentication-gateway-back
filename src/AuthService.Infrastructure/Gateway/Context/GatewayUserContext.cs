namespace AuthService.Infrastructure.Gateway.Context;

public sealed record GatewayUserContext(Guid UserId, string Username, string Email, string Status);

using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Gateway.Context;

public static class GatewayUserContextFactory
{
    public static GatewayUserContext Create(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new GatewayUserContext(
            user.Id,
            user.Username.Value,
            user.Email.Value,
            user.Status.ToString()
        );
    }
}

using System.Security.Claims;
using AuthService.Application.Abstractions.Security;

namespace AuthService.Api.Security;

public class CurrentUserService : ICurrentUser
{
    public Guid UserId { get; }

    public CurrentUserService(IHttpContextAccessor accessor)
    {
        var user = accessor.HttpContext?.User ?? throw new UnauthorizedAccessException();

        UserId = Guid.Parse(
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst("sub")?.Value
                ?? throw new UnauthorizedAccessException()
        );
    }
}

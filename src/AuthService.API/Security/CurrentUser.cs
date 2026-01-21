using AuthService.Application.Abstractions.Security;

namespace AuthService.Api.Security;

public sealed class CurrentUser : ICurrentUser
{
    public Guid UserId { get; }
    public string Username { get; }
    public string Email { get; }
    public string Status { get; }

    public CurrentUser(IHttpContextAccessor accessor)
    {
        var context =
            accessor.HttpContext ?? throw new UnauthorizedAccessException("No HttpContext");

        if (!context.Request.Cookies.TryGetValue("refresh_token", out var rawUserId))
            throw new UnauthorizedAccessException("User not authenticated");

        UserId = Guid.Parse(rawUserId);

        Username = context.Items["username"]?.ToString() ?? string.Empty;
        Email = context.Items["email"]?.ToString() ?? string.Empty;
        Status = context.Items["status"]?.ToString() ?? string.Empty;
    }
}

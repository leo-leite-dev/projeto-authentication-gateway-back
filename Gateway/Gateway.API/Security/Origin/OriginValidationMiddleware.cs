namespace Gateway.Api.Security.Origin;

public class OriginValidationMiddleware
{
    private readonly RequestDelegate _next;

    public OriginValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IOriginValidator originValidator)
    {
        if (!context.Request.Headers.TryGetValue("Origin", out var originValues))
        {
            await _next(context);
            return;
        }

        var origin = originValues.ToString();
        if (!string.IsNullOrEmpty(origin) && !originValidator.IsAllowed(origin))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Origin not allowed.");
            return;
        }

        await _next(context);
    }
}

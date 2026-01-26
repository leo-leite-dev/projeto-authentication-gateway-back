using Gateway.Api.Security.Origin;

namespace Gateway.API.Security.Csrf;

public sealed class CsrfProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOriginValidator _originValidator;

    public CsrfProtectionMiddleware(RequestDelegate next, IOriginValidator originValidator)
    {
        _next = next;
        _originValidator = originValidator;
    }

    public async Task Invoke(HttpContext context)
    {
        var method = context.Request.Method;
        var path = context.Request.Path;

        if (
            HttpMethods.IsPost(method)
            || HttpMethods.IsPut(method)
            || HttpMethods.IsPatch(method)
            || HttpMethods.IsDelete(method)
        )
        {
            var hasCookie = context.Request.Headers.ContainsKey("Cookie");

            if (hasCookie)
            {
                var origin = context.Request.Headers.Origin.ToString();
                var referer = context.Request.Headers.Referer.ToString();

                var isOriginValid =
                    !string.IsNullOrEmpty(origin) && _originValidator.IsAllowed(origin);

                var isRefererValid =
                    !string.IsNullOrEmpty(referer) && _originValidator.IsAllowed(referer);

                if (!isOriginValid && !isRefererValid)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("CSRF protection: invalid origin");
                    return;
                }
            }
        }

        await _next(context);
    }
}

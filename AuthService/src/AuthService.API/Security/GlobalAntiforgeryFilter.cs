using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.API.Security;

public class GlobalAntiforgeryFilter : IAsyncAuthorizationFilter
{
    private readonly IAntiforgery _antiforgery;

    public GlobalAntiforgeryFilter(IAntiforgery antiforgery)
    {
        _antiforgery = antiforgery;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            return;

        if (
            context
                .ActionDescriptor.EndpointMetadata.OfType<IgnoreAntiforgeryTokenAttribute>()
                .Any()
        )
            return;

        var method = context.HttpContext.Request.Method;

        if (
            HttpMethods.IsPost(method)
            || HttpMethods.IsPut(method)
            || HttpMethods.IsPatch(method)
            || HttpMethods.IsDelete(method)
        )
        {
            await _antiforgery.ValidateRequestAsync(context.HttpContext);
        }
    }
}

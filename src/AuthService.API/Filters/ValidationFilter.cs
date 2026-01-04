using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Api.Filters;

public sealed class ValidationFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(
                new
                {
                    error = "Requisição inválida.",
                    details = context
                        .ModelState.Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e => e.ErrorMessage)),
                }
            );
        }
    }
}

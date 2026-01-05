using System.Net;
using AuthService.Domain.Exceptions;

namespace AuthService.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            _logger.LogWarning(ex, "Erro de domínio");

            await WriteResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                "domain_error",
                ex.Message
            );
        }
        catch (ApplicationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação");

            await WriteResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                "validation_error",
                ex.Message
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Não autorizado");

            await WriteResponseAsync(
                context,
                HttpStatusCode.Unauthorized,
                "unauthorized",
                "Não autorizado."
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");

            await WriteResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "internal_error",
                "Erro interno no servidor."
            );
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string type,
        string message
    )
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new { type, message });
    }
}

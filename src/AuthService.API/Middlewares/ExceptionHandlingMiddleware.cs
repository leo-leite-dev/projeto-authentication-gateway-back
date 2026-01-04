using System.Net;
using System.Text.Json;
using AuthService.Domain.Exceptions;
using AppException = AuthService.Application.Common.Exceptions.ApplicationException;

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

            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Erro de aplicação");

            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Não autorizado");

            await WriteResponseAsync(context, HttpStatusCode.Unauthorized, "Não autorizado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado");

            await WriteResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Erro interno no servidor."
            );
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string message
    )
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new { error = message });

        await context.Response.WriteAsync(payload);
    }
}

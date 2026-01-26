using System.Security.Claims;

namespace Gateway.Api.Security.Revocation;

public sealed class TokenRevocationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpClient _httpClient;
    private readonly string _internalApiKey;

    public TokenRevocationMiddleware(
        RequestDelegate next,
        IHttpClientFactory factory,
        IConfiguration configuration
    )
    {
        _next = next;
        _httpClient = factory.CreateClient();
        _internalApiKey =
            configuration["InternalApi:ApiKey"]
            ?? throw new InvalidOperationException("Internal API Key not configured");
    }

    public async Task Invoke(HttpContext context)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tokenVersion = context.User.FindFirstValue("tv");

        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(tokenVersion))
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "http://authservice/internal/token/validate"
            );

            request.Headers.Add("X-Internal-Api-Key", _internalApiKey);

            request.Content = JsonContent.Create(
                new { UserId = Guid.Parse(userId), TokenVersion = int.Parse(tokenVersion) }
            );

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
        }

        await _next(context);
    }
}

using System.Net.Http.Headers;

namespace Gateway.Api.Forwarding;

public sealed record GatewayRoute(string Prefix, string BaseUrl);

public sealed class GatewayForwarder
{
    private readonly HttpClient _httpClient;
    private readonly IReadOnlyList<GatewayRoute> _routes;

    public GatewayForwarder(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        _routes =
            configuration.GetSection("Gateway:Routes").Get<List<GatewayRoute>>()
            ?? throw new InvalidOperationException("Gateway:Routes not configured");
    }

    public async Task<HttpResponseMessage> ForwardAsync(
        HttpRequest originalRequest,
        CancellationToken cancellationToken = default
    )
    {
        var targetUri = BuildTargetUri(originalRequest);

        var forwardedRequest = new HttpRequestMessage(
            new HttpMethod(originalRequest.Method),
            targetUri
        );

        CopyHeaders(originalRequest, forwardedRequest);
        await CopyBodyAsync(originalRequest, forwardedRequest);

        var authHeader = originalRequest.Headers["Authorization"].ToString();
        if (!string.IsNullOrWhiteSpace(authHeader))
            forwardedRequest.Headers.TryAddWithoutValidation("Authorization", authHeader);

        var cookieHeader = originalRequest.Headers["Cookie"].ToString();
        if (!string.IsNullOrWhiteSpace(cookieHeader))
            forwardedRequest.Headers.TryAddWithoutValidation("Cookie", cookieHeader);

        return await _httpClient.SendAsync(
            forwardedRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );
    }

    private Uri BuildTargetUri(HttpRequest request)
    {
        var path = request.Path.Value ?? "";

        var route = _routes
            .OrderByDescending(r => r.Prefix.Length)
            .FirstOrDefault(r => path.StartsWith(r.Prefix, StringComparison.OrdinalIgnoreCase));

        if (route is null)
            throw new InvalidOperationException($"No backend route found for path {path}");

        var downstreamPath = path.Substring(route.Prefix.Length);

        return new Uri($"{route.BaseUrl}{downstreamPath}{request.QueryString}");
    }

    private static void CopyHeaders(HttpRequest source, HttpRequestMessage destination)
    {
        foreach (var header in source.Headers)
        {
            if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                continue;
            if (header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                continue;
            if (header.Key.Equals("Cookie", StringComparison.OrdinalIgnoreCase))
                continue;

            if (!destination.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray()))
            {
                destination.Content?.Headers.TryAddWithoutValidation(
                    header.Key,
                    header.Value.ToArray()
                );
            }
        }
    }

    private static async Task CopyBodyAsync(HttpRequest source, HttpRequestMessage destination)
    {
        if (source.ContentLength is null || source.ContentLength == 0)
            return;

        destination.Content = new StreamContent(source.Body);

        if (!string.IsNullOrWhiteSpace(source.ContentType))
        {
            destination.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(
                source.ContentType
            );
        }
    }
}

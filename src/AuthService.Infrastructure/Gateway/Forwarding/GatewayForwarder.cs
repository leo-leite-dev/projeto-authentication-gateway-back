using System.Net.Http.Headers;
using AuthService.Infrastructure.Gateway.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Gateway.Forwarding;

public sealed class GatewayForwarder
{
    private readonly HttpClient _httpClient;
    private readonly string _conduitBaseUrl;

    public GatewayForwarder(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _conduitBaseUrl =
            configuration["Gateway:ConduitBaseUrl"]
            ?? throw new InvalidOperationException("Gateway:ConduitBaseUrl not configured");
    }

    public async Task<HttpResponseMessage> ForwardAsync(
        HttpRequest originalRequest,
        GatewayUserContext userContext,
        CancellationToken cancellationToken = default
    )
    {
        var targetUri = BuildTargetUri(originalRequest);

        var forwardedRequest = new HttpRequestMessage(
            new HttpMethod(originalRequest.Method),
            targetUri
        );

        CopyHeaders(originalRequest, forwardedRequest);
        InjectUserContext(forwardedRequest, userContext);
        await CopyBodyAsync(originalRequest, forwardedRequest);

        return await _httpClient.SendAsync(
            forwardedRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );
    }

    private Uri BuildTargetUri(HttpRequest request)
    {
        var uri = $"{_conduitBaseUrl}{request.Path}{request.QueryString}";
        return new Uri(uri);
    }

    private static void CopyHeaders(HttpRequest source, HttpRequestMessage destination)
    {
        foreach (var header in source.Headers)
        {
            if (header.Key.Equals("Host", StringComparison.OrdinalIgnoreCase))
                continue;

            destination.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }
    }

    private static void InjectUserContext(
        HttpRequestMessage request,
        GatewayUserContext userContext
    )
    {
        request.Headers.Add("X-User-Id", userContext.UserId.ToString());
        request.Headers.Add("X-Username", userContext.Username);
        request.Headers.Add("X-User-Email", userContext.Email);
        request.Headers.Add("X-User-Status", userContext.Status);
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

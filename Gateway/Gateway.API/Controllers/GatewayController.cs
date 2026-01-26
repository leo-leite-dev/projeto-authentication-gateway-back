using Gateway.API.Forwarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers;

[Authorize]
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/{**path}")]
public sealed class GatewayController : ControllerBase
{
    private readonly GatewayForwarder _forwarder;

    public GatewayController(GatewayForwarder forwarder)
    {
        _forwarder = forwarder;
    }

    [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
    public async Task Forward(CancellationToken cancellationToken)
    {
        var upstreamResponse = await _forwarder.ForwardAsync(Request, cancellationToken);

        Response.StatusCode = (int)upstreamResponse.StatusCode;

        foreach (var header in upstreamResponse.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }

        foreach (var header in upstreamResponse.Content.Headers)
        {
            Response.Headers[header.Key] = header.Value.ToArray();
        }

        Response.Headers.Remove("transfer-encoding");

        await upstreamResponse.Content.CopyToAsync(Response.Body, cancellationToken);
    }
}

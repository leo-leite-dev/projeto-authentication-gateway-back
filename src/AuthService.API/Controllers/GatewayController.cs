using AuthService.Infrastructure.Gateway.Forwarding;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("{**path}")]
public sealed class GatewayController : ControllerBase
{
    private readonly GatewayForwarder _forwarder;

    public GatewayController(GatewayForwarder forwarder)
    {
        _forwarder = forwarder;
    }

    [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
    public async Task<IActionResult> Forward(CancellationToken cancellationToken)
    {
        var response = await _forwarder.ForwardAsync(Request, cancellationToken);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, body);
    }
}

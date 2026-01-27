using Gateway.Api.Forwarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[AllowAnonymous]
[Route("users")]
public sealed class PublicGatewayController : ControllerBase
{
    private readonly GatewayForwarder _forwarder;

    public PublicGatewayController(GatewayForwarder forwarder)
    {
        _forwarder = forwarder;
    }

    [HttpPost]
    public async Task<IActionResult> Register(CancellationToken ct) => await Forward(ct);

    [HttpPost("login")]
    public async Task<IActionResult> Login(CancellationToken ct) => await Forward(ct);

    private async Task<IActionResult> Forward(CancellationToken ct)
    {
        var response = await _forwarder.ForwardAsync(Request, ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        return StatusCode((int)response.StatusCode, body);
    }
}

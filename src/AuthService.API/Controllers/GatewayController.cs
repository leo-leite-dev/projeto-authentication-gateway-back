using AuthService.Application.Abstractions.Repositories;
using AuthService.Infrastructure.Gateway.Context;
using AuthService.Infrastructure.Gateway.Forwarding;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("{**path}")]
public sealed class GatewayController : ControllerBase
{
    private readonly GatewayForwarder _forwarder;
    private readonly IUserRepository _userRepository;

    public GatewayController(GatewayForwarder forwarder, IUserRepository userRepository)
    {
        _forwarder = forwarder;
        _userRepository = userRepository;
    }

    [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
    public async Task<IActionResult> Forward(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var userIdRaw))
            return Unauthorized();

        if (!Guid.TryParse(userIdRaw, out var userId))
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
            return Unauthorized();

        var userContext = GatewayUserContextFactory.Create(user);

        var response = await _forwarder.ForwardAsync(Request, userContext, cancellationToken);

        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        return StatusCode((int)response.StatusCode, body);
    }
}

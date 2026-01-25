using AuthService.Api.Contracts.Auth.RefreshToken;
using AuthService.Api.Security.Cookies;
using AuthService.Application.UseCases.Auth.Logout;
using AuthService.Application.UseCases.Auth.RefreshTokens;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("user")]
public sealed class UserSessionController : ControllerBase
{
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly AuthCookieService _cookieService;

    public UserSessionController(
        RefreshTokenUseCase refreshTokenUseCase,
        LogoutUseCase logoutUseCase,
        AuthCookieService cookieService
    )
    {
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _cookieService = cookieService;
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh(
        CancellationToken cancellationToken
    )
    {
        var result = await _refreshTokenUseCase.ExecuteAsync(cancellationToken);

        return Ok(new RefreshTokenResponse(result.RefreshedAt));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await _logoutUseCase.ExecuteAsync(cancellationToken);

        _cookieService.Clear(Response);

        return NoContent();
    }
}

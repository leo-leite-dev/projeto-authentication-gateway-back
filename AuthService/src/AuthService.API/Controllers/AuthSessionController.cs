using AuthService.Api.Security.Cookies;
using AuthService.Application.UseCases.Auth.Logout;
using AuthService.Application.UseCases.Auth.RefreshTokens;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("user")]
[Authorize]
public sealed class AuthSessionController : ControllerBase
{
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly AuthCookieService _cookieService;
    private readonly IAntiforgery _antiforgery;

    public AuthSessionController(
        RefreshTokenUseCase refreshTokenUseCase,
        LogoutUseCase logoutUseCase,
        AuthCookieService cookieService,
        IAntiforgery antiforgery
    )
    {
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _cookieService = cookieService;
        _antiforgery = antiforgery;
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized();

        var command = new RefreshTokenCommand(refreshToken);

        var result = await _refreshTokenUseCase.ExecuteAsync(command, cancellationToken);

        Response.Cookies.Append(
            "refresh_token",
            result.RefreshToken,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7),
            }
        );

        _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new { accessToken = result.AccessToken });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refresh_token"];

        if (string.IsNullOrWhiteSpace(refreshToken))
            return NoContent();

        var command = new LogoutCommand(refreshToken);

        await _logoutUseCase.ExecuteAsync(command, cancellationToken);

        _cookieService.Clear(Response);

        _antiforgery.GetAndStoreTokens(HttpContext);

        return NoContent();
    }
}

using AuthService.Api.Configuration;
using AuthService.Api.Contracts.Auth;
using AuthService.Api.Security.Claims;
using AuthService.Api.Security.Cookies;
using AuthService.Application.UseCases.Auth.Login;
using AuthService.Application.UseCases.Auth.Logout;
using AuthService.Application.UseCases.Auth.RefreshTokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly AuthCookieService _cookieService;
    private readonly ApiOptions _apiOptions;

    public AuthController(
        LoginUseCase loginUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        LogoutUseCase logoutUseCase,
        AuthCookieService cookieService,
        IOptions<ApiOptions> apiOptions
    )
    {
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _cookieService = cookieService;
        _apiOptions = apiOptions.Value;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await _loginUseCase.ExecuteAsync(
            new LoginCommand(request.Login, request.Password),
            cancellationToken
        );

        _cookieService.AppendRefreshToken(Response, result.RefreshToken);

        return Ok(new LoginResponse(result.AccessToken, result.AccessTokenExpiresAt));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh(
        CancellationToken cancellationToken
    )
    {
        if (!Request.Cookies.TryGetValue(_apiOptions.RefreshTokenCookieName, out var refreshToken))
            return Unauthorized();

        var result = await _refreshTokenUseCase.ExecuteAsync(
            new RefreshTokenCommand(refreshToken),
            cancellationToken
        );

        return Ok(new RefreshTokenResponse(result.AccessToken, result.AccessTokenExpiresAt));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        await _logoutUseCase.ExecuteAsync(new LogoutCommand(userId), cancellationToken);

        _cookieService.RemoveTokens(Response);

        return NoContent();
    }
}

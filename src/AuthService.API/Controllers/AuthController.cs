using AuthService.Api.Contracts.Auth.Login;
using AuthService.Api.Contracts.Auth.RefreshToken;
using AuthService.Api.Contracts.Auth.Register;
using AuthService.Api.Security.Cookies;
using AuthService.Application.UseCases.Auth.Login;
using AuthService.Application.UseCases.Auth.Logout;
using AuthService.Application.UseCases.Auth.RefreshTokens;
using AuthService.Application.UseCases.Auth.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase;
    private readonly LogoutUseCase _logoutUseCase;
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly AuthCookieService _cookieService;

    public AuthController(
        LoginUseCase loginUseCase,
        RefreshTokenUseCase refreshTokenUseCase,
        LogoutUseCase logoutUseCase,
        RegisterUserUseCase registerUserUseCase,
        AuthCookieService cookieService
    )
    {
        _loginUseCase = loginUseCase;
        _refreshTokenUseCase = refreshTokenUseCase;
        _logoutUseCase = logoutUseCase;
        _registerUserUseCase = registerUserUseCase;
        _cookieService = cookieService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterUserResponse>> Register(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await _registerUserUseCase.ExecuteAsync(
            new RegisterUserCommand(request.Username, request.Email, request.Password),
            cancellationToken
        );

        return Created(
            string.Empty,
            new RegisterUserResponse(result.UserId, result.Username, result.Email, result.Status)
        );
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

        _cookieService.AppendRefreshToken(Response, result.UserId.ToString());

        return Ok(new LoginResponse(result.UserId, result.Username, result.Email, result.Status));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
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

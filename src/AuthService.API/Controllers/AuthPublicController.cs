using AuthService.Api.Contracts.Auth.Login;
using AuthService.Api.Contracts.Auth.Register;
using AuthService.Api.Mappers;
using AuthService.Api.Security.Cookies;
using AuthService.Application.UseCases.Auth.Login;
using AuthService.Application.UseCases.Auth.Register;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class AuthPublicController : ControllerBase
{
    private readonly LoginUseCase _loginUseCase;
    private readonly RegisterUserUseCase _registerUserUseCase;
    private readonly AuthCookieService _cookieService;

    public AuthPublicController(
        LoginUseCase loginUseCase,
        RegisterUserUseCase registerUserUseCase,
        AuthCookieService cookieService
    )
    {
        _loginUseCase = loginUseCase;
        _registerUserUseCase = registerUserUseCase;
        _cookieService = cookieService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<RegisterUserResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await _registerUserUseCase.ExecuteAsync(
            request.ToCommand(),
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
        var result = await _loginUseCase.ExecuteAsync(request.ToCommand(), cancellationToken);

        _cookieService.AppendRefreshToken(Response, result.UserId.ToString());

        return Ok(new LoginResponse(result.UserId, result.Username, result.Email, result.Status));
    }
}

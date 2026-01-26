using AuthService.Application.UseCases.Auth.ValidateToken;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("internal/token")]
public sealed class InternalTokenController : ControllerBase
{
    private readonly ValidateTokenVersionUseCase _useCase;

    public InternalTokenController(ValidateTokenVersionUseCase useCase) => _useCase = useCase;

    [HttpPost("validate")]
    public async Task<IActionResult> Validate([FromBody] TokenValidationRequest request) =>
        await _useCase.ExecuteAsync(request.UserId, request.TokenVersion) ? Ok() : Unauthorized();
}

public sealed record TokenValidationRequest(Guid UserId, int TokenVersion);

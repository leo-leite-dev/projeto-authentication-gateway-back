using AuthService.Api.Contracts.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("me")]
    public ActionResult<GetCurrentUserResponse> GetMe()
    {
        var userId = Guid.Parse(User.FindFirst("sub")!.Value);
        var username = User.Identity!.Name!;
        var email = User.FindFirst("email")!.Value;

        return Ok(new GetCurrentUserResponse(userId, username, email));
    }
}

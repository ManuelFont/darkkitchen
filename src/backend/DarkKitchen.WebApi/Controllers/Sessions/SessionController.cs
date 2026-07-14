using DarkKitchen.Application.Services.Sessions;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Sessions;
using DarkKitchen.WebApi.Responses.Sessions;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Sessions;

[ApiController]
[Route("sessions")]
public sealed class SessionController(ISessionService sessionService) : ControllerBase
{
    private readonly ISessionService _sessionService = sessionService;

    [HttpPost]
    public IActionResult Login(LoginRequest request)
    {
        var result = _sessionService.Login(request.ToDto());
        return Created($"sessions/{result.Token}", new LoginResponse(result.Token, result.RoleName));
    }

    [HttpDelete("{token}")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    public IActionResult Logout(Guid token)
    {
        _sessionService.Logout(token);
        return NoContent();
    }
}

using DarkKitchen.Application.Services.Users;
using DarkKitchen.Domain.Entities;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Users;
using DarkKitchen.WebApi.Responses.Users;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Users;

[ApiController]
[Route("users")]
public sealed class UserController(IUserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost("register")]
    public IActionResult Register(CreateUserRequest request)
    {
        var id = _userService.Register(request.ToDto());
        return Created($"users/{id}", new CreateUserResponse(id));
    }

    [HttpGet]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanGetUsers)]
    public IActionResult GetUsers([FromQuery] GetUsersRequest request)
    {
        var result = _userService.GetUsers(request.ToDto());
        return Ok(result.Select(UserSummaryResponse.FromDto).ToList());
    }

    [HttpPost]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanCreateUser)]
    public IActionResult Create(CreateAdminUserRequest request)
    {
        var result = _userService.CreateByAdmin(request.ToDto());
        return Created($"users/{result.Id}", UserSummaryResponse.FromDto(result));
    }

    [HttpPut("{id}")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanUpdateUser)]
    public IActionResult Update([FromRoute] Guid id, UpdateUserRequest request)
    {
        var requester = (User)HttpContext.Items[HttpContextItemKey.UserLogged]!;
        var result = _userService.Update(requester.Id, id, request.ToDto());
        return Ok(UserSummaryResponse.FromDto(result));
    }

    [HttpDelete("{id}")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanDeleteUser)]
    public IActionResult Delete([FromRoute] Guid id)
    {
        var requester = (User)HttpContext.Items[HttpContextItemKey.UserLogged]!;
        _userService.Delete(requester.Id, id);
        return NoContent();
    }
}

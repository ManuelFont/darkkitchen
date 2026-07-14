using DarkKitchen.Application.Services.Users.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Requests.Users;

public sealed record GetUsersRequest
{
    [FromQuery]
    public string? Search { get; init; }

    [FromQuery]
    public string? Role { get; init; }

    public GetUsersDto ToDto() => new()
    {
        Search = Search,
        Role = Role
    };
}

namespace DarkKitchen.Application.Services.Users.Dtos;

public sealed record GetUsersDto
{
    public string? Search { get; init; }
    public string? Role { get; init; }
}

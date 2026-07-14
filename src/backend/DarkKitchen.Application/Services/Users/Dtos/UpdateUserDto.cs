namespace DarkKitchen.Application.Services.Users.Dtos;

public sealed record UpdateUserDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Password { get; init; }
    public string Phone { get; init; } = string.Empty;
    public int RoleId { get; init; }
}

namespace DarkKitchen.Application.Services.Sessions.Dtos;

public sealed record LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

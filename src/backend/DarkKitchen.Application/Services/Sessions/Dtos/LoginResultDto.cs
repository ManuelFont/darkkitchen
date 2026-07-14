namespace DarkKitchen.Application.Services.Sessions.Dtos;

public sealed record LoginResultDto
{
    public Guid Token { get; init; }
    public string RoleName { get; init; } = string.Empty;
}

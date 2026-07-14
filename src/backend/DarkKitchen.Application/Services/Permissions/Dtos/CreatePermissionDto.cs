namespace DarkKitchen.Application.Services.Permissions.Dtos;

public sealed record CreatePermissionDto
{
    public int PermissionId { get; init; }
    public string PermissionName { get; init; } = string.Empty;
}

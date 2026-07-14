using DarkKitchen.Application.Services.Users.Dtos;

namespace DarkKitchen.WebApi.Responses.Users;

public sealed record UserSummaryResponse
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public int RoleId { get; init; }
    public string RoleName { get; init; } = string.Empty;

    public static UserSummaryResponse FromDto(UserSummaryDto dto) => new()
    {
        Id = dto.Id,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        Email = dto.Email,
        Phone = dto.Phone,
        RoleId = dto.RoleId,
        RoleName = dto.RoleName
    };
}

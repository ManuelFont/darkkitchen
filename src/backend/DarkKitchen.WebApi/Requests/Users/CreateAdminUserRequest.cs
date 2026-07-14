using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Users.Dtos;

namespace DarkKitchen.WebApi.Requests.Users;

public sealed record CreateAdminUserRequest
{
    [Required]
    public required string FirstName { get; init; }

    [Required]
    public required string LastName { get; init; }

    [Required]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }

    [Required]
    public required string Phone { get; init; }

    [Required]
    public required int RoleId { get; init; }

    public CreateAdminUserDto ToDto() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Password = Password,
        Phone = Phone,
        RoleId = RoleId
    };
}

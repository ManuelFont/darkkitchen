using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Users.Dtos;

namespace DarkKitchen.WebApi.Requests.Users;

public sealed record UpdateUserRequest
{
    [Required]
    public required string FirstName { get; init; }

    [Required]
    public required string LastName { get; init; }

    [Required]
    public required string Email { get; init; }

    public string? Password { get; init; }

    [Required]
    public required string Phone { get; init; }

    [Required]
    public required int RoleId { get; init; }

    public UpdateUserDto ToDto() => new()
    {
        FirstName = FirstName,
        LastName = LastName,
        Email = Email,
        Password = Password,
        Phone = Phone,
        RoleId = RoleId
    };
}

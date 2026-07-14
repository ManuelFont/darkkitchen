using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Sessions.Dtos;

namespace DarkKitchen.WebApi.Requests.Sessions;

public sealed record LoginRequest
{
    [Required]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }

    public LoginDto ToDto() => new()
    {
        Email = Email,
        Password = Password
    };
}

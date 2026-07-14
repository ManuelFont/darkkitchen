using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.WebApi.Requests.Orders;

public sealed record AddressRequest
{
    [Required]
    public required string Street { get; init; }

    [Required]
    public int DoorNumber { get; init; }

    public string? Apartment { get; init; }
}

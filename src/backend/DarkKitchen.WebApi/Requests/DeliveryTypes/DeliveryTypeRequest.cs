using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.DeliveryTypes;

namespace DarkKitchen.WebApi.Requests.DeliveryTypes;

public sealed record DeliveryTypeRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public decimal Cost { get; init; }

    public CreateDeliveryTypeDto ToDto() => new()
    {
        Name = Name,
        Cost = Cost
    };
}

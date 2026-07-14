using System.ComponentModel.DataAnnotations;
using DarkKitchen.Application.Services.Orders.Dtos;

namespace DarkKitchen.WebApi.Requests.Orders;

public sealed record CreateOrderRequest
{
    [Required]
    public Guid DeliveryTypeId { get; init; }

    [Required]
    public required AddressRequest Address { get; init; }

    [Required]
    [MinLength(1)]
    public required IReadOnlyList<CreateOrderItemRequest> Items { get; init; }

    public CreateOrderDto ToDto(Guid clientId) => new()
    {
        ClientId = clientId,
        DeliveryTypeId = DeliveryTypeId,
        Street = Address.Street,
        DoorNumber = Address.DoorNumber,
        Apartment = Address.Apartment,
        Items = Items
            .Select(item => new CreateOrderItemDto
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            })
            .ToList()
    };
}

using DarkKitchen.Application.Services.Orders.Dtos;

namespace DarkKitchen.WebApi.Responses.Orders;

public sealed record OrderDetailItemResponse
{
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
}

public sealed record OrderDetailResponse
{
    public Guid OrderId { get; init; }
    public string ClientFirstName { get; init; } = string.Empty;
    public string ClientLastName { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Subtotal { get; init; }
    public decimal DeliveryCost { get; init; }
    public decimal Total { get; init; }
    public Guid DeliveryTypeId { get; init; }
    public string DeliveryTypeName { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public int DoorNumber { get; init; }
    public string? Apartment { get; init; }
    public IReadOnlyList<OrderDetailItemResponse> Items { get; init; } = [];

    public static OrderDetailResponse FromDto(OrderDetailDto dto) => new()
    {
        OrderId = dto.OrderId,
        ClientFirstName = dto.ClientFirstName,
        ClientLastName = dto.ClientLastName,
        ClientEmail = dto.ClientEmail,
        CreatedAt = dto.CreatedAt,
        Status = dto.Status.ToString(),
        Subtotal = dto.Subtotal,
        DeliveryCost = dto.DeliveryCost,
        Total = dto.Total,
        DeliveryTypeId = dto.DeliveryTypeId,
        DeliveryTypeName = dto.DeliveryTypeName,
        Street = dto.Street,
        DoorNumber = dto.DoorNumber,
        Apartment = dto.Apartment,
        Items = dto.Items
            .Select(i => new OrderDetailItemResponse
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Subtotal
            })
            .ToList()
    };
}

using DarkKitchen.Application.Services.Orders.Dtos;

namespace DarkKitchen.WebApi.Responses.Orders;

public sealed record OrderItemResponse
{
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}

public sealed record OrderListItemResponse
{
    public Guid OrderId { get; init; }
    public string ClientFirstName { get; init; } = string.Empty;
    public string ClientLastName { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public IReadOnlyList<OrderItemResponse> Items { get; init; } = [];

    public static OrderListItemResponse FromDto(OrderListItemDto dto) => new()
    {
        OrderId = dto.OrderId,
        ClientFirstName = dto.ClientFirstName,
        ClientLastName = dto.ClientLastName,
        ClientEmail = dto.ClientEmail,
        CreatedAt = dto.CreatedAt,
        Status = dto.Status.ToString(),
        Total = dto.Total,
        Items = dto.Items
            .Select(i => new OrderItemResponse
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                Price = i.Price
            })
            .ToList()
    };
}

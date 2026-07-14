using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record OrderListItemDto
{
    public Guid OrderId { get; init; }
    public string ClientFirstName { get; init; } = string.Empty;
    public string ClientLastName { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public decimal Total { get; init; }
    public IReadOnlyList<OrderListItemProductDto> Items { get; init; } = [];
}

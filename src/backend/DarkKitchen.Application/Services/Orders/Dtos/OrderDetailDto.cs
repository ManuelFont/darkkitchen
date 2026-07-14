using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record OrderDetailDto
{
    public Guid OrderId { get; init; }
    public string ClientFirstName { get; init; } = string.Empty;
    public string ClientLastName { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public OrderStatus Status { get; init; }
    public decimal Subtotal { get; init; }
    public decimal DeliveryCost { get; init; }
    public decimal Total { get; init; }
    public Guid DeliveryTypeId { get; init; }
    public string DeliveryTypeName { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public int DoorNumber { get; init; }
    public string? Apartment { get; init; }
    public IReadOnlyList<OrderDetailItemDto> Items { get; init; } = [];
}

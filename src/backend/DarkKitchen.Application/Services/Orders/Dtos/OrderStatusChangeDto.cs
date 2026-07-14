using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record OrderStatusChangeDto
{
    public Guid OrderId { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime StatusChangedAt { get; init; }
}

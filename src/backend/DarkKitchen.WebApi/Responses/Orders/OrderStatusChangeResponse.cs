using DarkKitchen.Application.Services.Orders.Dtos;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.WebApi.Responses.Orders;

public sealed record OrderStatusChangeResponse(Guid OrderId, OrderStatus Status, DateTime StatusChangedAt)
{
    public static OrderStatusChangeResponse FromDto(OrderStatusChangeDto dto) =>
        new(dto.OrderId, dto.Status, dto.StatusChangedAt);
}

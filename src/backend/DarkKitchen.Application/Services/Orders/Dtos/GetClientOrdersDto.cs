using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record GetClientOrdersDto
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
    public OrderStatus? Status { get; init; }
}

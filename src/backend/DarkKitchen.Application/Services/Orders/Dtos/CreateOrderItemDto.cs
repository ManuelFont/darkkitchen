namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record CreateOrderItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}

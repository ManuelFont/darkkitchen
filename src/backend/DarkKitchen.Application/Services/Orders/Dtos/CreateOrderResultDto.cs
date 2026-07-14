namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record CreateOrderResultDto
{
    public Guid OrderId { get; init; }
    public Guid ClientId { get; init; }
    public decimal Subtotal { get; init; }
    public decimal DeliveryCost { get; init; }
    public decimal Total { get; init; }
}

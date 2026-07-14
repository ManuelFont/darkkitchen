namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record OrderDetailItemDto
{
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
}

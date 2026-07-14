namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record OrderListItemProductDto
{
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal Price { get; init; }
}

namespace DarkKitchen.Application.Services.Orders.Dtos;

public sealed record CreateOrderDto
{
    public Guid ClientId { get; init; }
    public Guid DeliveryTypeId { get; init; }
    public required string Street { get; init; }
    public int DoorNumber { get; init; }
    public string? Apartment { get; init; }
    public required IReadOnlyList<CreateOrderItemDto> Items { get; init; }
}

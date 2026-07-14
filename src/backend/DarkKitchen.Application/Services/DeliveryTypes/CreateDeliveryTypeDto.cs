namespace DarkKitchen.Application.Services.DeliveryTypes;

public sealed record CreateDeliveryTypeDto
{
    public required string Name { get; init; }
    public decimal Cost { get; init; }
}

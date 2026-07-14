namespace DarkKitchen.Application.Services.DeliveryTypes;

public sealed record DeliveryTypeReadDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Cost { get; init; }
}

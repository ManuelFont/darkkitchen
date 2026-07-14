using DarkKitchen.Application.Services.DeliveryTypes;

namespace DarkKitchen.WebApi.Responses.DeliveryTypes;

public sealed record DeliveryTypeResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Cost { get; init; }

    public static DeliveryTypeResponse FromDto(DeliveryTypeReadDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Cost = dto.Cost
    };
}

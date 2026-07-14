using DarkKitchen.Application.Services.Orders.Dtos;

namespace DarkKitchen.WebApi.Responses.Orders;

public sealed record ClientOrderSummaryResponse
{
    public Guid OrderId { get; init; }
    public string ClientFirstName { get; init; } = string.Empty;
    public string ClientLastName { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public int ProductCount { get; init; }

    public static ClientOrderSummaryResponse FromDto(ClientOrderSummaryDto dto) => new()
    {
        OrderId = dto.OrderId,
        ClientFirstName = dto.ClientFirstName,
        ClientLastName = dto.ClientLastName,
        ClientEmail = dto.ClientEmail,
        CreatedAt = dto.CreatedAt,
        Status = dto.Status.ToString(),
        Total = dto.Total,
        ProductCount = dto.ProductCount
    };
}

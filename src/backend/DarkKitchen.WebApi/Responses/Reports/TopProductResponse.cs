using DarkKitchen.Application.Services.Reports.Dtos;

namespace DarkKitchen.WebApi.Responses.Reports;

/// <summary>Top-sold product report row. Image links are intentionally omitted until the images feature is implemented.</summary>
public sealed record TopProductResponse
{
    public Guid ProductCode { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Quantity { get; init; }

    public static TopProductResponse FromDto(TopProductDto dto) => new()
    {
        ProductCode = dto.ProductId,
        Name = dto.Name,
        Quantity = dto.Quantity
    };
}

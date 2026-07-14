namespace DarkKitchen.Application.Services.Reports.Dtos;

public sealed record GetTopProductsDto
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

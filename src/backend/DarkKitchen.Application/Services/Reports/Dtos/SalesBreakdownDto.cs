namespace DarkKitchen.Application.Services.Reports.Dtos;

public sealed record SalesBreakdownDto
{
    public required decimal Items { get; init; }
    public required decimal Delivery { get; init; }
    public required decimal Iva { get; init; }
    public required decimal Total { get; init; }
}

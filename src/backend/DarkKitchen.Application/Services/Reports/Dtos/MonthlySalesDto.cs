namespace DarkKitchen.Application.Services.Reports.Dtos;

public sealed record MonthlySalesDto
{
    public required int Year { get; init; }
    public required int Month { get; init; }
    public required IReadOnlyList<ClientSalesDto> Clients { get; init; } = [];
    public required SalesBreakdownDto Subtotal { get; init; }
}

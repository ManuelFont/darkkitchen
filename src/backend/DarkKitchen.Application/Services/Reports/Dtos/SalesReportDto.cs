namespace DarkKitchen.Application.Services.Reports.Dtos;

public sealed record SalesReportDto
{
    public required IReadOnlyList<MonthlySalesDto> Months { get; init; } = [];
    public required SalesBreakdownDto GrandTotal { get; init; }
}

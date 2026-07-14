namespace DarkKitchen.Application.Services.Reports.Dtos;

public sealed record GetSalesReportDto
{
    public DateTime? DateFrom { get; init; }
    public DateTime? DateTo { get; init; }
}

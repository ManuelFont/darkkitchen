using DarkKitchen.Application.Services.Reports.Dtos;

namespace DarkKitchen.WebApi.Responses.Reports;

public sealed record SalesBreakdownResponse
{
    public decimal Items { get; init; }
    public decimal Delivery { get; init; }
    public decimal Iva { get; init; }
    public decimal Total { get; init; }

    public static SalesBreakdownResponse FromDto(SalesBreakdownDto dto) => new()
    {
        Items = dto.Items,
        Delivery = dto.Delivery,
        Iva = dto.Iva,
        Total = dto.Total
    };
}

public sealed record ClientSalesResponse
{
    public Guid ClientId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public required SalesBreakdownResponse Amounts { get; init; }

    public static ClientSalesResponse FromDto(ClientSalesDto dto) => new()
    {
        ClientId = dto.ClientId,
        ClientName = dto.ClientName,
        Amounts = SalesBreakdownResponse.FromDto(dto.Amounts)
    };
}

public sealed record MonthlySalesResponse
{
    public string Period { get; init; } = string.Empty;
    public int Year { get; init; }
    public int Month { get; init; }
    public IReadOnlyList<ClientSalesResponse> Clients { get; init; } = [];
    public required SalesBreakdownResponse Subtotal { get; init; }

    public static MonthlySalesResponse FromDto(MonthlySalesDto dto) => new()
    {
        Period = $"{dto.Year}-{dto.Month:D2}",
        Year = dto.Year,
        Month = dto.Month,
        Clients = dto.Clients.Select(ClientSalesResponse.FromDto).ToList(),
        Subtotal = SalesBreakdownResponse.FromDto(dto.Subtotal)
    };
}

public sealed record SalesReportResponse
{
    public IReadOnlyList<MonthlySalesResponse> Months { get; init; } = [];
    public required SalesBreakdownResponse GrandTotal { get; init; }

    public static SalesReportResponse FromDto(SalesReportDto dto) => new()
    {
        Months = dto.Months.Select(MonthlySalesResponse.FromDto).ToList(),
        GrandTotal = SalesBreakdownResponse.FromDto(dto.GrandTotal)
    };
}

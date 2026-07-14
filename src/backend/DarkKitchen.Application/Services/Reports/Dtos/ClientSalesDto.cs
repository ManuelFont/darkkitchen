namespace DarkKitchen.Application.Services.Reports.Dtos;

public sealed record ClientSalesDto
{
    public required Guid ClientId { get; init; }
    public required string ClientName { get; init; } = string.Empty;
    public required SalesBreakdownDto Amounts { get; init; }
}

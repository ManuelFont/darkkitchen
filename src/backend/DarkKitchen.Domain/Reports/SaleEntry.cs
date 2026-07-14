namespace DarkKitchen.Domain.Reports;

public sealed record SaleEntry(
    DateTime CreatedAt,
    Guid ClientId,
    string ClientName,
    decimal ItemsTotal,
    decimal DeliveryCost,
    decimal Total);

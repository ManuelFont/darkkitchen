namespace DarkKitchen.Domain.Reports;

public sealed record TopSoldProduct(Guid ProductId, string ProductName, int QuantitySold);

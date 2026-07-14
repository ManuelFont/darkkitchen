namespace DarkKitchen.Application.Services.Reports.Dtos;

public sealed record TopProductDto
{
    public required Guid ProductId { get; init; }
    public required string Name { get; init; } = string.Empty;
    public required int Quantity { get; init; }
}

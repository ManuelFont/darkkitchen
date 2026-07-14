namespace DarkKitchen.Application.Services.Promotions;

public sealed record PromotionCreateDto
{
    public required string Name { get; init; }
    public required decimal DiscountPercentage { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
}

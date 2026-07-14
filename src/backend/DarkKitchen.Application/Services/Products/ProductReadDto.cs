using DarkKitchen.Application.Services.Categories;
using DarkKitchen.Application.Services.Promotions;

namespace DarkKitchen.Application.Services.Products;

public sealed record ProductReadDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; } = string.Empty;
    public required string Description { get; init; } = string.Empty;
    public required IReadOnlyList<string> ImagesUrls { get; init; }
    public required decimal Price { get; init; }
    public required Guid CategoryId { get; init; }
    public required CategoryReadDto Category { get; init; }
    public required PromotionReadDto? ActivePromotion { get; init; }
    public required IEnumerable<PromotionReadDto> Promotions { get; init; } = [];
}

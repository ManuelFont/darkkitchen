using DarkKitchen.Application.Services.Products;
using DarkKitchen.WebApi.Responses.Categories;
using DarkKitchen.WebApi.Responses.Promotions;

namespace DarkKitchen.WebApi.Responses.Products;

public sealed record ProductResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public required IReadOnlyList<string> ImagesUrls { get; init; }
    public decimal Price { get; init; }
    public Guid CategoryId { get; init; }
    public required CategoryResponse Category { get; init; }
    public required PromotionResponse? ActivePromotion { get; init; }
    public required IEnumerable<PromotionResponse> Promotions { get; init; } = [];

    public static ProductResponse FromDto(ProductReadDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        ImagesUrls = dto.ImagesUrls,
        Price = dto.Price,
        CategoryId = dto.CategoryId,
        Category = CategoryResponse.FromDto(dto.Category),
        ActivePromotion = dto.ActivePromotion != null
            ? PromotionResponse.FromDto(dto.ActivePromotion)
            : null,
        Promotions = dto.Promotions.Select(PromotionResponse.FromDto).ToList()
    };
}

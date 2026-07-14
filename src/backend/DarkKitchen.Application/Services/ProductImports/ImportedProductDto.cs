namespace DarkKitchen.Application.Services.ProductImports;

public sealed record ImportedProductDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<string> ImagesUrls { get; init; }
    public required decimal Price { get; init; }
    public required Guid CategoryId { get; init; }
}

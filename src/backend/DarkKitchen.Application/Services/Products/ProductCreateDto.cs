namespace DarkKitchen.Application.Services.Products;

public sealed record ProductCreateDto
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required IReadOnlyList<string> ImagesUrls { get; init; }
    public required decimal Price { get; init; }
    public required Guid CategoryId { get; init; }
}

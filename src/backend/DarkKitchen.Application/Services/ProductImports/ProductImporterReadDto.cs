namespace DarkKitchen.Application.Services.ProductImports;

public sealed record ProductImporterReadDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
}

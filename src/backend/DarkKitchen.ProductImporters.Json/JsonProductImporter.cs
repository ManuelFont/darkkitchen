using System.Text.Json;
using DarkKitchen.Application.Services.ProductImports;

namespace DarkKitchen.ProductImporters.Json;

public sealed class JsonProductImporter : IProductImporter
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public string Id => "json";
    public string Name => "JSON products";
    public string Description => "Imports products from a JSON array.";

    public bool CanImport(string fileName, string? contentType)
    {
        return Path.GetExtension(fileName).Equals(".json", StringComparison.OrdinalIgnoreCase)
            || string.Equals(contentType, "application/json", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyList<ImportedProductDto>> ImportAsync(
        Stream source,
        CancellationToken cancellationToken = default)
    {
        var products = await JsonSerializer.DeserializeAsync<List<JsonProductDto>>(
            source,
            SerializerOptions,
            cancellationToken);

        if(products == null)
        {
            return [];
        }

        return products.Select(product => new ImportedProductDto
        {
            Name = product.Name,
            Description = product.Description,
            ImagesUrls = product.ImagesUrls,
            Price = product.Price,
            CategoryId = product.CategoryId
        }).ToList();
    }

    private sealed record JsonProductDto
    {
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public IReadOnlyList<string> ImagesUrls { get; init; } = [];
        public decimal Price { get; init; }
        public Guid CategoryId { get; init; }
    }
}

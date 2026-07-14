namespace DarkKitchen.Application.Services.ProductImports;

public interface IProductImporter
{
    string Id { get; }
    string Name { get; }
    string Description { get; }
    bool CanImport(string fileName, string? contentType);
    Task<IReadOnlyList<ImportedProductDto>> ImportAsync(Stream source, CancellationToken cancellationToken = default);
}

namespace DarkKitchen.Application.Services.ProductImports;

public interface IProductImportService
{
    IReadOnlyList<ProductImporterReadDto> GetImporters();
    Task<ProductImportResultDto> ImportAsync(
        string importerId,
        string fileName,
        string? contentType,
        Stream source,
        CancellationToken cancellationToken = default);
}

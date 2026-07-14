using DarkKitchen.Application.Services.Products;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.ProductImports;

public sealed class ProductImportService(
    IProductImporterProvider importerProvider,
    IProductService productService,
    IProductRepository productRepository) : IProductImportService
{
    private readonly IProductImporterProvider _importerProvider = importerProvider;
    private readonly IProductService _productService = productService;
    private readonly IProductRepository _productRepository = productRepository;

    public IReadOnlyList<ProductImporterReadDto> GetImporters()
    {
        return _importerProvider.GetImporters()
            .Select(importer => new ProductImporterReadDto
            {
                Id = importer.Id,
                Name = importer.Name,
                Description = importer.Description
            })
            .ToList();
    }

    public async Task<ProductImportResultDto> ImportAsync(
        string importerId,
        string fileName,
        string? contentType,
        Stream source,
        CancellationToken cancellationToken = default)
    {
        var importer = _importerProvider.GetById(importerId);
        if(importer == null)
        {
            throw new ResourceNotFoundException("Product importer", importerId);
        }

        if(!importer.CanImport(fileName, contentType))
        {
            throw new InvalidArgumentException($"Importer {importer.Id} cannot import file {fileName}.");
        }

        IReadOnlyList<ImportedProductDto> importedProducts;

        try
        {
            importedProducts = await importer.ImportAsync(source, cancellationToken);
        }
        catch(OperationCanceledException)
        {
            throw;
        }
        catch(Exception exception)
        {
            throw new InvalidArgumentException($"Could not import products: {exception.Message}");
        }

        var issues = new List<ProductImportIssueDto>();
        var created = 0;
        var skippedDuplicates = 0;

        foreach(var importedProduct in importedProducts)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if(_productRepository.Exists(product => product.Name == importedProduct.Name))
            {
                skippedDuplicates++;
                issues.Add(new ProductImportIssueDto
                {
                    Name = importedProduct.Name,
                    Message = "Product already exists."
                });
                continue;
            }

            try
            {
                _productService.Create(new ProductCreateDto
                {
                    Name = importedProduct.Name,
                    Description = importedProduct.Description,
                    ImagesUrls = importedProduct.ImagesUrls,
                    Price = importedProduct.Price,
                    CategoryId = importedProduct.CategoryId
                });
                created++;
            }
            catch(DomainException exception)
            {
                issues.Add(new ProductImportIssueDto
                {
                    Name = importedProduct.Name,
                    Message = exception.Message
                });
            }
        }

        return new ProductImportResultDto
        {
            ImporterId = importer.Id,
            Total = importedProducts.Count,
            Created = created,
            SkippedDuplicates = skippedDuplicates,
            Failed = issues.Count - skippedDuplicates,
            Issues = issues
        };
    }
}

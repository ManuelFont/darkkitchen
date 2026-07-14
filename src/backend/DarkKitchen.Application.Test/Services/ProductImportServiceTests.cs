using System.Linq.Expressions;
using System.Text;
using DarkKitchen.Application.Services.ProductImports;
using DarkKitchen.Application.Services.Products;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class ProductImportServiceTests
{
    private Mock<IProductImporterProvider> _providerMock = null!;
    private Mock<IProductImporter> _importerMock = null!;
    private Mock<IProductService> _productServiceMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private ProductImportService _service = null!;

    [TestInitialize]
    public void SetUp()
    {
        _providerMock = new Mock<IProductImporterProvider>();
        _importerMock = new Mock<IProductImporter>();
        _productServiceMock = new Mock<IProductService>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _service = new ProductImportService(
            _providerMock.Object,
            _productServiceMock.Object,
            _productRepositoryMock.Object);

        _importerMock.SetupGet(importer => importer.Id).Returns("json");
        _importerMock.SetupGet(importer => importer.Name).Returns("JSON products");
        _importerMock.SetupGet(importer => importer.Description).Returns("Imports products from JSON.");
        _importerMock.Setup(importer => importer.CanImport("products.json", "application/json")).Returns(true);
        _providerMock.Setup(provider => provider.GetById("json")).Returns(_importerMock.Object);
    }

    [TestMethod]
    public void GetImporters_ReturnsAvailableImporters()
    {
        _providerMock.Setup(provider => provider.GetImporters()).Returns([_importerMock.Object]);

        var result = _service.GetImporters();

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("json", result[0].Id);
        Assert.AreEqual("JSON products", result[0].Name);
    }

    [TestMethod]
    public async Task ImportAsync_CreatesProducts_WhenImportedProductsAreValid()
    {
        var categoryId = Guid.NewGuid();
        var products = new List<ImportedProductDto>
        {
            Product("Pizza", categoryId),
            Product("Burger", categoryId)
        };
        _importerMock
            .Setup(importer => importer.ImportAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        _productRepositoryMock
            .Setup(repository => repository.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);

        var result = await _service.ImportAsync(
            "json",
            "products.json",
            "application/json",
            StreamWithContent("[]"));

        Assert.AreEqual(2, result.Total);
        Assert.AreEqual(2, result.Created);
        Assert.AreEqual(0, result.SkippedDuplicates);
        Assert.AreEqual(0, result.Failed);
        _productServiceMock.Verify(service => service.Create(It.IsAny<ProductCreateDto>()), Times.Exactly(2));
    }

    [TestMethod]
    public async Task ImportAsync_SkipsDuplicateProducts()
    {
        var duplicate = Product("Pizza", Guid.NewGuid());
        _importerMock
            .Setup(importer => importer.ImportAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([duplicate]);
        _productRepositoryMock
            .Setup(repository => repository.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(true);

        var result = await _service.ImportAsync(
            "json",
            "products.json",
            "application/json",
            StreamWithContent("[]"));

        Assert.AreEqual(1, result.Total);
        Assert.AreEqual(0, result.Created);
        Assert.AreEqual(1, result.SkippedDuplicates);
        Assert.AreEqual(0, result.Failed);
        Assert.AreEqual("Pizza", result.Issues[0].Name);
        _productServiceMock.Verify(service => service.Create(It.IsAny<ProductCreateDto>()), Times.Never);
    }

    [TestMethod]
    public async Task ImportAsync_ReportsProductCreationErrorsAndContinues()
    {
        var valid = Product("Pizza", Guid.NewGuid());
        var invalid = Product("Burger", Guid.NewGuid());
        _importerMock
            .Setup(importer => importer.ImportAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([invalid, valid]);
        _productRepositoryMock
            .Setup(repository => repository.Exists(It.IsAny<Expression<Func<Product, bool>>>()))
            .Returns(false);
        _productServiceMock
            .SetupSequence(service => service.Create(It.IsAny<ProductCreateDto>()))
            .Throws(new ResourceNotFoundException("Category not found", invalid.CategoryId))
            .Returns(new ProductReadDto
            {
                Id = Guid.NewGuid(),
                Name = valid.Name,
                Description = valid.Description,
                ImagesUrls = valid.ImagesUrls,
                Price = valid.Price,
                CategoryId = valid.CategoryId,
                Category = new() { Id = valid.CategoryId, Name = "Food", Description = "Food category" },
                ActivePromotion = null,
                Promotions = []
            });

        var result = await _service.ImportAsync(
            "json",
            "products.json",
            "application/json",
            StreamWithContent("[]"));

        Assert.AreEqual(2, result.Total);
        Assert.AreEqual(1, result.Created);
        Assert.AreEqual(1, result.Failed);
        Assert.AreEqual("Burger", result.Issues[0].Name);
        _productServiceMock.Verify(service => service.Create(It.IsAny<ProductCreateDto>()), Times.Exactly(2));
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public async Task ImportAsync_Throws_WhenImporterDoesNotExist()
    {
        _providerMock.Setup(provider => provider.GetById("missing")).Returns((IProductImporter?)null);

        await _service.ImportAsync(
            "missing",
            "products.json",
            "application/json",
            StreamWithContent("[]"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public async Task ImportAsync_Throws_WhenImporterCannotImportFile()
    {
        _importerMock.Setup(importer => importer.CanImport("products.txt", "text/plain")).Returns(false);

        await _service.ImportAsync(
            "json",
            "products.txt",
            "text/plain",
            StreamWithContent("[]"));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public async Task ImportAsync_ThrowsInvalidArgument_WhenImporterFails()
    {
        _importerMock
            .Setup(importer => importer.ImportAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new FormatException("Invalid JSON."));

        await _service.ImportAsync(
            "json",
            "products.json",
            "application/json",
            StreamWithContent("[]"));
    }

    [TestMethod]
    [ExpectedException(typeof(OperationCanceledException))]
    public async Task ImportAsync_Rethrows_WhenOperationIsCanceled()
    {
        _importerMock
            .Setup(importer => importer.ImportAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        await _service.ImportAsync(
            "json",
            "products.json",
            "application/json",
            StreamWithContent("[]"));
    }

    private static ImportedProductDto Product(string name, Guid categoryId) => new()
    {
        Name = name,
        Description = "Imported product",
        ImagesUrls = ["https://example.com/product.jpg"],
        Price = 10,
        CategoryId = categoryId
    };

    private static MemoryStream StreamWithContent(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}

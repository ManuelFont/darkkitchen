using System.Text;
using DarkKitchen.Application.Services.ProductImports;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.ProductImports;
using DarkKitchen.WebApi.Responses.ProductImports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class ProductImportControllerTests
{
    private Mock<IProductImportService> _serviceMock = null!;
    private ProductImportController _controller = null!;

    [TestInitialize]
    public void SetUp()
    {
        _serviceMock = new Mock<IProductImportService>();
        _controller = new ProductImportController(_serviceMock.Object);
    }

    [TestMethod]
    public void GetImporters_ReturnsAvailableImporters()
    {
        _serviceMock.Setup(service => service.GetImporters()).Returns([
            new ProductImporterReadDto
            {
                Id = "json",
                Name = "JSON products",
                Description = "Imports products from JSON."
            }

        ]);

        var result = (OkObjectResult)_controller.GetImporters().Result!;
        var response = (List<ProductImporterResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(1, response.Count);
        Assert.AreEqual("json", response[0].Id);
    }

    [TestMethod]
    public async Task Import_ReturnsImportResult()
    {
        var file = FormFile("products.json", "application/json", "[]");
        var dto = new ProductImportResultDto
        {
            ImporterId = "json",
            Total = 1,
            Created = 1,
            SkippedDuplicates = 0,
            Failed = 0,
            Issues = []
        };
        _serviceMock
            .Setup(service => service.ImportAsync(
                "json",
                "products.json",
                "application/json",
                It.IsAny<Stream>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = (OkObjectResult)(await _controller.Import("json", file, CancellationToken.None)).Result!;
        var response = (ProductImportResultResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(1, response.Created);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public async Task Import_Throws_WhenFileIsEmpty()
    {
        var file = FormFile("products.json", "application/json", string.Empty);

        await _controller.Import("json", file, CancellationToken.None);
    }

    private static IFormFile FormFile(string fileName, string contentType, string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return new FormFile(stream, 0, stream.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}

using System.Text;
using DarkKitchen.ProductImporters.Json;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class JsonProductImporterTests
{
    [TestMethod]
    public void Properties_ReturnImporterMetadata()
    {
        var importer = new JsonProductImporter();

        Assert.AreEqual("json", importer.Id);
        Assert.AreEqual("JSON products", importer.Name);
        Assert.AreEqual("Imports products from a JSON array.", importer.Description);
    }

    [TestMethod]
    public void CanImport_ReturnsTrue_ForJsonFile()
    {
        var importer = new JsonProductImporter();

        Assert.IsTrue(importer.CanImport("products.json", null));
    }

    [TestMethod]
    public void CanImport_ReturnsTrue_ForJsonFileWithUppercaseExtension()
    {
        var importer = new JsonProductImporter();

        Assert.IsTrue(importer.CanImport("PRODUCTS.JSON", null));
    }

    [TestMethod]
    public void CanImport_ReturnsTrue_ForJsonContentType()
    {
        var importer = new JsonProductImporter();

        Assert.IsTrue(importer.CanImport("products.txt", "application/json"));
    }

    [TestMethod]
    public void CanImport_ReturnsFalse_WhenFileAndContentTypeAreNotJson()
    {
        var importer = new JsonProductImporter();

        Assert.IsFalse(importer.CanImport("products.txt", "text/plain"));
    }

    [TestMethod]
    public async Task ImportAsync_ParsesJsonProducts()
    {
        var categoryId = Guid.NewGuid();
        var json = $$"""
        [
          {
            "name": "Pizza",
            "description": "Pizza mozzarella",
            "price": 350,
            "categoryId": "{{categoryId}}",
            "imagesUrls": ["images/pizza.jpg"]
          }
        ]
        """;
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var importer = new JsonProductImporter();

        var result = await importer.ImportAsync(stream);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Pizza", result[0].Name);
        Assert.AreEqual("Pizza mozzarella", result[0].Description);
        Assert.AreEqual(350, result[0].Price);
        Assert.AreEqual(categoryId, result[0].CategoryId);
        CollectionAssert.AreEqual(new[] { "images/pizza.jpg" }, result[0].ImagesUrls.ToList());
    }

    [TestMethod]
    public async Task ImportAsync_ReturnsEmptyList_WhenJsonIsNull()
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("null"));
        var importer = new JsonProductImporter();

        var result = await importer.ImportAsync(stream);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ImportAsync_UsesDefaultValues_WhenOptionalJsonFieldsAreMissing()
    {
        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("[{}]"));
        var importer = new JsonProductImporter();

        var result = await importer.ImportAsync(stream);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(string.Empty, result[0].Name);
        Assert.AreEqual(string.Empty, result[0].Description);
        Assert.AreEqual(0, result[0].Price);
        Assert.AreEqual(Guid.Empty, result[0].CategoryId);
        Assert.AreEqual(0, result[0].ImagesUrls.Count);
    }
}

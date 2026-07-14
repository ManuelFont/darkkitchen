using DarkKitchen.Application.Services.ProductImports;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class ProductImporterProviderTests
{
    [TestMethod]
    public void GetImporters_ReturnsEmptyList_WhenPluginDirectoryDoesNotExist()
    {
        var provider = new ProductImporterProvider(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

        var result = provider.GetImporters();

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetImporters_LoadsImporterFromPluginAssembly()
    {
        var pluginPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(pluginPath);
        File.Copy(
            typeof(TestProductImporter).Assembly.Location,
            Path.Combine(pluginPath, "TestProductImporter.dll"));
        var provider = new ProductImporterProvider(pluginPath);

        var result = provider.GetImporters();

        Assert.IsTrue(result.Any(importer => importer.Id == "test-importer"));
    }

    [TestMethod]
    public void GetById_ReturnsNull_WhenImporterIdIsWhitespace()
    {
        var provider = new ProductImporterProvider(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));

        var result = provider.GetById(" ");

        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetById_ReturnsImporter_WhenIdMatchesDifferentCasing()
    {
        var pluginPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(pluginPath);
        File.Copy(
            typeof(TestProductImporter).Assembly.Location,
            Path.Combine(pluginPath, "TestProductImporter.dll"));
        var provider = new ProductImporterProvider(pluginPath);

        var result = provider.GetById("TEST-IMPORTER");

        Assert.IsNotNull(result);
        Assert.AreEqual("test-importer", result.Id);
    }

    [TestMethod]
    public void GetById_ReturnsNull_WhenImporterDoesNotExist()
    {
        var pluginPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(pluginPath);
        File.Copy(
            typeof(TestProductImporter).Assembly.Location,
            Path.Combine(pluginPath, "TestProductImporter.dll"));
        var provider = new ProductImporterProvider(pluginPath);

        var result = provider.GetById("missing");

        Assert.IsNull(result);
    }

    public sealed class TestProductImporter : IProductImporter
    {
        public string Id => "test-importer";
        public string Name => "Test importer";
        public string Description => "Importer used by provider tests.";

        public bool CanImport(string fileName, string? contentType) => true;

        public Task<IReadOnlyList<ImportedProductDto>> ImportAsync(
            Stream source,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<ImportedProductDto>>([]);
        }
    }
}

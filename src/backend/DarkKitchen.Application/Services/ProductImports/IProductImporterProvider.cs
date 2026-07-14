namespace DarkKitchen.Application.Services.ProductImports;

public interface IProductImporterProvider
{
    IReadOnlyList<IProductImporter> GetImporters();
    IProductImporter? GetById(string importerId);
}

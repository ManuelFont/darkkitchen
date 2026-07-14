using System.Reflection;
using System.Runtime.Loader;

namespace DarkKitchen.Application.Services.ProductImports;

public sealed class ProductImporterProvider : IProductImporterProvider
{
    private readonly Lazy<IReadOnlyList<IProductImporter>> _importers;
    private readonly string _pluginsPath;

    public ProductImporterProvider()
        : this(Path.Combine(AppContext.BaseDirectory, "Plugins"))
    {
    }

    public ProductImporterProvider(string pluginsPath)
    {
        _pluginsPath = pluginsPath;
        _importers = new Lazy<IReadOnlyList<IProductImporter>>(LoadImporters);
    }

    public IReadOnlyList<IProductImporter> GetImporters() => _importers.Value;

    public IProductImporter? GetById(string importerId)
    {
        if(string.IsNullOrWhiteSpace(importerId))
        {
            return null;
        }

        return GetImporters()
            .FirstOrDefault(importer => string.Equals(importer.Id, importerId, StringComparison.OrdinalIgnoreCase));
    }

    private IReadOnlyList<IProductImporter> LoadImporters()
    {
        if(!Directory.Exists(_pluginsPath))
        {
            return [];
        }

        return Directory
            .EnumerateFiles(_pluginsPath, "*.dll", SearchOption.TopDirectoryOnly)
            .SelectMany(LoadImportersFromAssembly)
            .GroupBy(importer => importer.Id, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(importer => importer.Name)
            .ToList();
    }

    private static IEnumerable<IProductImporter> LoadImportersFromAssembly(string assemblyPath)
    {
        Assembly assembly;
        try
        {
            assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(assemblyPath));
        }
        catch
        {
            yield break;
        }

        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch(ReflectionTypeLoadException exception)
        {
            types = exception.Types.Where(type => type != null).Cast<Type>().ToArray();
        }

        foreach(var type in types)
        {
            if(type.IsAbstract || type.IsInterface || !typeof(IProductImporter).IsAssignableFrom(type))
            {
                continue;
            }

            IProductImporter? importer;
            try
            {
                importer = Activator.CreateInstance(type) as IProductImporter;
            }
            catch
            {
                continue;
            }

            if(importer != null)
            {
                yield return importer;
            }
        }
    }
}

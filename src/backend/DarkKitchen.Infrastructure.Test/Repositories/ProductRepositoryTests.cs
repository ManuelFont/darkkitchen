using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class ProductRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IProductRepository _repository = null!;
    private Product _product = null!;
    private ProductCategory _category = null!;

    [TestInitialize]
    public void Setup()
    {
        _category = new ProductCategory("Congelados", "Comida en heladera");
        _product = new Product("name", "description", 3, _category, ["https://example.com/image.jpg"]);

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new SqlDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new ProductRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsProduct()
    {
        _repository.Add(_product);

        var found = _context.Products.Find(_product.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("name", found.Name);
        Assert.AreEqual("description", found.Description);
        CollectionAssert.AreEqual(_product.ImagesUrls.ToList(), found.ImagesUrls.ToList());
    }

    [TestMethod]
    [ExpectedException(typeof(DbUpdateException))]
    public void Add_ThrowsExceptionIfProductAlreadyExists()
    {
        _repository.Add(_product);
        _repository.Add(_product);
    }

    [TestMethod]
    public void GetById_ReturnsProduct()
    {
        _repository.Add(_product);

        var found = _repository.GetById(_product.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(_product.Id, found.Id);
        Assert.AreEqual("name", found.Name);
        Assert.AreEqual("description", found.Description);
        CollectionAssert.AreEqual(_product.ImagesUrls.ToList(), found.ImagesUrls.ToList());
    }

    [TestMethod]
    public void GetById_ReturnsNullIfProductDoesNotExist()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_ReturnsAllProducts()
    {
        var product2 = new Product("name two", "description two", 5, _category, ["https://example.com/image.jpg"]);

        _repository.Add(_product);
        _repository.Add(product2);

        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(17, result.Count());
        Assert.IsTrue(result.Any(p => p.Name == "name"));
        Assert.IsTrue(result.Any(p => p.Name == "name two"));
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyListIfNoProductsExist()
    {
        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(15, result.Count());
        Assert.IsTrue(result.All(product => product.ImagesUrls.Count == 1));
    }

    [TestMethod]
    public void Exists_ReturnsTrueIfProductExists()
    {
        _repository.Add(_product);

        var result = _repository.Exists(p => p.Name == "name");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_ReturnsFalseIfProductDoesNotExist()
    {
        var result = _repository.Exists(p => p.Name == "name");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsInOrder_ReturnsTrueIfProductIsInOrder()
    {
        var orderedProductId = Guid.Parse("20000000-0000-0000-0000-000000000001");

        var result = _repository.IsInOrder(orderedProductId);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsInOrder_ReturnsFalseIfProductIsNotInOrder()
    {
        _repository.Add(_product);

        var result = _repository.IsInOrder(_product.Id);

        Assert.IsFalse(result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Update_ThrowsExceptionIfProductDoesNotExist()
    {
        _repository.Update(_product);
    }

    [TestMethod]
    public void Delete_DeletesProduct()
    {
        _repository.Add(_product);
        _repository.Delete(_product.Id);

        var found = _context.Products.Find(_product.Id);
        Assert.IsNull(found);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Delete_ThrowsExceptionIfProductDoesNotExist()
    {
        _repository.Delete(_product.Id);
    }

    [TestMethod]
    public void GetByName_ReturnsMatchingProduct()
    {
        _repository.Add(_product);

        var found = _repository.GetByName("name");

        Assert.AreEqual(1, found.Count);
        Assert.AreEqual(_product.Id, found[0].Id);
        Assert.AreEqual("name", found[0].Name);
    }

    [TestMethod]
    public void GetByName_MatchesPartialTermCaseInsensitivelyAndTrimmed()
    {
        _repository.Add(_product);

        var found = _repository.GetByName("  AM ");

        Assert.AreEqual(1, found.Count);
        Assert.AreEqual(_product.Id, found[0].Id);
    }

    [TestMethod]
    public void GetByName_ReturnsEmptyIfNoProductMatches()
    {
        var result = _repository.GetByName("nonexistent");
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByCategory_ReturnsProductsInCategory()
    {
        var otherCategory = new ProductCategory("Frescos", "Comida fresca");
        var otherProduct = new Product("other", "other description", 5, otherCategory, ["https://example.com/image.jpg"]);

        _repository.Add(_product);
        _repository.Add(otherProduct);

        var result = _repository.GetByCategory(_category.CategoryId);

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("name", result[0].Name);
    }

    [TestMethod]
    public void GetByCategory_ReturnsEmptyListIfNoneMatch()
    {
        var otherCategory = new ProductCategory("Frescos", "Comida fresca");

        _repository.Add(_product);

        var result = _repository.GetByCategory(otherCategory.CategoryId);

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
}

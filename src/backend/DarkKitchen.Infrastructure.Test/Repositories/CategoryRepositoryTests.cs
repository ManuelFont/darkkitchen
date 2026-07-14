using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class CategoryRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private ICategoryRepository _repository = null!;
    private ProductCategory _category = null!;

    [TestInitialize]
    public void Setup()
    {
        _category = new ProductCategory("Congelados", "Comida en heladera");

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new SqlDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new CategoryRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsCategory()
    {
        _repository.Add(_category);

        var found = _context.ProductCategories.Find(_category.CategoryId);
        Assert.IsNotNull(found);
        Assert.AreEqual("Congelados", found.Name);
        Assert.AreEqual("Comida en heladera", found.Description);
    }

    [TestMethod]
    [ExpectedException(typeof(DbUpdateException))]
    public void Add_ThrowsExceptionIfCategoryAlreadyExists()
    {
        _repository.Add(_category);
        _repository.Add(_category);
    }

    [TestMethod]
    public void GetById_ReturnsCategory()
    {
        _repository.Add(_category);

        var found = _repository.GetById(_category.CategoryId);

        Assert.IsNotNull(found);
        Assert.AreEqual(_category.CategoryId, found.CategoryId);
        Assert.AreEqual("Congelados", found.Name);
    }

    [TestMethod]
    public void GetById_ReturnsNullIfCategoryDoesNotExist()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_ReturnsAllCategories()
    {
        var category2 = new ProductCategory("Frescos", "Comida fresca");

        _repository.Add(_category);
        _repository.Add(category2);

        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(7, result.Count());
        Assert.IsTrue(result.Any(c => c.Name == "Congelados"));
        Assert.IsTrue(result.Any(c => c.Name == "Frescos"));
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyListIfNoCategoriesExist()
    {
        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count());
    }

    [TestMethod]
    public void Exists_ReturnsTrueIfCategoryExists()
    {
        _repository.Add(_category);

        var result = _repository.Exists(c => c.Name == "Congelados");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_ReturnsFalseIfCategoryDoesNotExist()
    {
        var result = _repository.Exists(c => c.Name == "Congelados");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Update_UpdatesCategory()
    {
        _repository.Add(_category);

        _category.Update("Lácteos", _category.Description);
        _repository.Update(_category);

        var found = _context.ProductCategories.Find(_category.CategoryId);
        Assert.IsNotNull(found);
        Assert.AreEqual("Lácteos", found.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Update_ThrowsExceptionIfCategoryDoesNotExist()
    {
        _repository.Update(_category);
    }

    [TestMethod]
    public void Delete_DeletesCategory()
    {
        _repository.Add(_category);
        _repository.Delete(_category.CategoryId);

        var found = _context.ProductCategories.Find(_category.CategoryId);
        Assert.IsNull(found);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Delete_ThrowsExceptionIfCategoryDoesNotExist()
    {
        _repository.Delete(_category.CategoryId);
    }
}

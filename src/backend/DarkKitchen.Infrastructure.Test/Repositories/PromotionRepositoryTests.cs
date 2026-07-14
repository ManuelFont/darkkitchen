using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class PromotionRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IPromotionRepository _repository = null!;
    private Promotion _promotion = null!;

    [TestInitialize]
    public void Setup()
    {
        _promotion = new Promotion("Summer Sale", 0.20m, DateTime.Today, DateTime.Today.AddDays(30));

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new SqlDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new PromotionRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsPromotion()
    {
        _repository.Add(_promotion);

        var found = _context.Promotions.Find(_promotion.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("Summer Sale", found.Name);
        Assert.AreEqual(0.20m, found.DiscountPercentage);
    }

    [TestMethod]
    [ExpectedException(typeof(DbUpdateException))]
    public void Add_ThrowsExceptionIfPromotionAlreadyExists()
    {
        _repository.Add(_promotion);
        _repository.Add(_promotion);
    }

    [TestMethod]
    public void GetById_ReturnsPromotion()
    {
        _repository.Add(_promotion);

        var found = _repository.GetById(_promotion.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(_promotion.Id, found.Id);
        Assert.AreEqual("Summer Sale", found.Name);
    }

    [TestMethod]
    public void GetById_ReturnsNullIfPromotionDoesNotExist()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_ReturnsAllPromotions()
    {
        var promotion2 = new Promotion("Winter Sale", 0.15m, DateTime.Today, DateTime.Today.AddDays(15));

        _repository.Add(_promotion);
        _repository.Add(promotion2);

        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(5, result.Count());
        Assert.IsTrue(result.Any(p => p.Name == "Summer Sale"));
        Assert.IsTrue(result.Any(p => p.Name == "Winter Sale"));
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyListIfNoPromotionsExist()
    {
        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(3, result.Count());
    }

    [TestMethod]
    public void Exists_ReturnsTrueIfPromotionExists()
    {
        _repository.Add(_promotion);

        var result = _repository.Exists(p => p.Name == "Summer Sale");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_ReturnsFalseIfPromotionDoesNotExist()
    {
        var result = _repository.Exists(p => p.Name == "Summer Sale");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Update_UpdatesPromotion()
    {
        _repository.Add(_promotion);

        _promotion.Update("Spring Sale", 0.30m, _promotion.StartDate, _promotion.EndDate);
        _repository.Update(_promotion);

        var found = _context.Promotions.Find(_promotion.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("Spring Sale", found.Name);
        Assert.AreEqual(0.30m, found.DiscountPercentage);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_ThrowsExceptionIfPromotionDoesNotExist()
    {
        _repository.Update(_promotion);
    }

    [TestMethod]
    public void Delete_DeletesPromotion()
    {
        _repository.Add(_promotion);
        _repository.Delete(_promotion.Id);

        var found = _context.Promotions.Find(_promotion.Id);
        Assert.IsNull(found);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Delete_ThrowsExceptionIfPromotionDoesNotExist()
    {
        _repository.Delete(_promotion.Id);
    }

    [TestMethod]
    public void GetByProduct_ReturnsPromotions_WhenProductExists()
    {
        var category = new ProductCategory("Congelados", "Comida heladera");
        var product = new Product("Hamburguesa", "Rica hamburguesa", 10m, category, ["https://example.com/image.jpg"]);
        var promotion = new Promotion("Test Promo", 0.2m, DateTime.Today, DateTime.Today.AddDays(30));
        product.AddPromotion(promotion);
        _context.Add(product);
        _context.SaveChanges();

        var result = _repository.GetByProduct(product.Id);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(promotion.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByProduct_ReturnsEmptyList_WhenProductDoesNotExist()
    {
        var result = _repository.GetByProduct(Guid.NewGuid());

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByCategory_ReturnsPromotions_WhenCategoryHasProducts()
    {
        var category = new ProductCategory("Congelados", "Comida heladera");
        var product = new Product("Hamburguesa", "Rica hamburguesa", 10m, category, ["https://example.com/image.jpg"]);
        var promotion = new Promotion("Test Promo", 0.2m, DateTime.Today, DateTime.Today.AddDays(30));
        product.AddPromotion(promotion);
        _context.Add(product);
        _context.SaveChanges();

        var result = _repository.GetByCategory(category.CategoryId);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(promotion.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByCategory_ReturnsEmptyList_WhenCategoryDoesNotExist()
    {
        var result = _repository.GetByCategory(Guid.NewGuid());

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }
}

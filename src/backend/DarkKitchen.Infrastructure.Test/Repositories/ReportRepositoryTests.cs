using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.Reports;
using DarkKitchen.Domain.Validators;
using DarkKitchen.Domain.ValueObjects;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class ReportRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IReportRepository _repository = null!;
    private ProductCategory _category = null!;
    private Product _burger = null!;
    private Product _pizza = null!;
    private Product _sushi = null!;
    private User _user = null!;
    private DeliveryType _deliveryType = null!;

    [TestInitialize]
    public void Setup()
    {
        _category = new ProductCategory("Congelados", "Comida en heladera");
        _burger = new Product("Burger", "Carne casera", 10m, _category, ["https://example.com/image.jpg"]);
        _pizza = new Product("Pizza", "Muzzarella artesanal", 12m, _category, ["https://example.com/image.jpg"]);
        _sushi = new Product("Sushi", "Salmon fresco", 15m, _category, ["https://example.com/image.jpg"]);
        _user = new User("John", "Doe", "john@test.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));
        _deliveryType = new DeliveryType("Test delivery", 3m);

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new SqlDbContext(options);
        _context.Database.EnsureCreated();

        _context.ProductCategories.Add(_category);
        _context.Products.AddRange(_burger, _pizza, _sushi);
        _context.Users.Add(_user);
        _context.DeliveryTypes.Add(_deliveryType);
        _context.SaveChanges();

        _repository = new ReportRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void GetTopSoldProducts_OrdersProductsByQuantityDescending()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 2), new OrderItem(_pizza, 5));

        var result = _repository.GetTopSoldProducts(RangeFrom(order), RangeTo(order), 5);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(_pizza.Id, result[0].ProductId);
        Assert.AreEqual(5, result[0].QuantitySold);
        Assert.AreEqual(_burger.Id, result[1].ProductId);
        Assert.AreEqual(2, result[1].QuantitySold);
    }

    [TestMethod]
    public void GetTopSoldProducts_SumsQuantitiesAcrossDeliveredOrders()
    {
        var firstOrder = AddDeliveredOrder(new OrderItem(_burger, 2));
        var secondOrder = AddDeliveredOrder(new OrderItem(_burger, 3));

        var result = _repository.GetTopSoldProducts(RangeFrom(firstOrder), RangeTo(secondOrder), 5);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_burger.Id, result[0].ProductId);
        Assert.AreEqual(5, result[0].QuantitySold);
    }

    [TestMethod]
    public void GetTopSoldProducts_PopulatesProductName()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetTopSoldProducts(RangeFrom(order), RangeTo(order), 5);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Burger", result[0].ProductName);
    }

    [TestMethod]
    public void GetTopSoldProducts_ExcludesNonDeliveredOrders()
    {
        var deliveredOrder = AddDeliveredOrder(new OrderItem(_burger, 1));
        AddPendingOrder(new OrderItem(_pizza, 10));

        var result = _repository.GetTopSoldProducts(RangeFrom(deliveredOrder), RangeTo(deliveredOrder), 5);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_burger.Id, result[0].ProductId);
    }

    [TestMethod]
    public void GetTopSoldProducts_ExcludesOrdersOutsideDateRange()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetTopSoldProducts(
            order.CreatedAt.AddSeconds(10),
            order.CreatedAt.AddSeconds(20),
            5);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopSoldProducts_DateRangeBoundsAreInclusive()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetTopSoldProducts(order.CreatedAt, order.CreatedAt, 5);

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetTopSoldProducts_CapsResultAtTop()
    {
        var order = AddDeliveredOrder(
            new OrderItem(_burger, 3),
            new OrderItem(_pizza, 2),
            new OrderItem(_sushi, 1));

        var result = _repository.GetTopSoldProducts(RangeFrom(order), RangeTo(order), 2);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(_burger.Id, result[0].ProductId);
        Assert.AreEqual(_pizza.Id, result[1].ProductId);
    }

    [TestMethod]
    public void GetTopSoldProducts_WithNullRange_IncludesAllDeliveredOrders()
    {
        AddDeliveredOrder(new OrderItem(_pizza, 5));

        var result = _repository.GetTopSoldProducts(null, null, 50);

        var pizza = result.SingleOrDefault(p => p.ProductId == _pizza.Id);
        Assert.IsNotNull(pizza, "Delivered orders should be included when no range is given");
        Assert.AreEqual(5, pizza.QuantitySold);
    }

    [TestMethod]
    public void GetTopSoldProducts_WithOnlyDateFrom_FiltersLowerBoundOnly()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetTopSoldProducts(RangeFrom(order), null, 5);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_burger.Id, result[0].ProductId);
    }

    [TestMethod]
    public void GetTopSoldProducts_WhenNoDeliveredOrdersInRange_ReturnsEmptyList()
    {
        AddPendingOrder(new OrderItem(_burger, 1));

        var result = _repository.GetTopSoldProducts(
            DateTime.UtcNow.AddSeconds(-30),
            DateTime.UtcNow.AddSeconds(30),
            5);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetDeliveredSales_ProjectsClientAndCreatedAt()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetDeliveredSales(RangeFrom(order), RangeTo(order));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_user.Id, result[0].ClientId);
        Assert.AreEqual("John Doe", result[0].ClientName);
        Assert.AreEqual(order.CreatedAt, result[0].CreatedAt);
    }

    [TestMethod]
    public void GetDeliveredSales_ComputesTotalsFromDomainPricing()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 2));

        var result = _repository.GetDeliveredSales(RangeFrom(order), RangeTo(order));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(20m, result[0].ItemsTotal);
        Assert.AreEqual(3m, result[0].DeliveryCost);
        Assert.AreEqual(28.06m, result[0].Total);
    }

    [TestMethod]
    public void GetDeliveredSales_AppliesActivePromotionDiscount()
    {
        var promotion = new Promotion("Mega Promo", 0.5m, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
        _context.Promotions.Add(promotion);
        _context.SaveChanges();
        _burger.AddPromotion(promotion);
        _context.SaveChanges();
        var order = AddDeliveredOrder(new OrderItem(_burger, 2));

        var result = _repository.GetDeliveredSales(RangeFrom(order), RangeTo(order));

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(10m, result[0].ItemsTotal);
        Assert.AreEqual(3m, result[0].DeliveryCost);
        Assert.AreEqual(15.86m, result[0].Total);
    }

    [TestMethod]
    public void GetDeliveredSales_ExcludesNonDeliveredOrders()
    {
        var deliveredOrder = AddDeliveredOrder(new OrderItem(_burger, 1));
        AddPendingOrder(new OrderItem(_pizza, 10));

        var result = _repository.GetDeliveredSales(RangeFrom(deliveredOrder), RangeTo(deliveredOrder));

        Assert.AreEqual(1, result.Count);
    }

    [TestMethod]
    public void GetDeliveredSales_ExcludesOrdersOutsideDateRange()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetDeliveredSales(order.CreatedAt.AddSeconds(10), order.CreatedAt.AddSeconds(20));

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetDeliveredSales_WithNullRange_IncludesAllDeliveredOrders()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetDeliveredSales(null, null);

        Assert.IsTrue(result.Any(s => s.ClientId == _user.Id && s.CreatedAt == order.CreatedAt));
        Assert.IsTrue(result.Any(s => s.CreatedAt < order.CreatedAt), "Seeded delivered orders should be included when no range is given");
    }

    [TestMethod]
    public void GetDeliveredSales_WithOnlyDateFrom_FiltersLowerBoundOnly()
    {
        var order = AddDeliveredOrder(new OrderItem(_burger, 1));

        var result = _repository.GetDeliveredSales(RangeFrom(order), null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_user.Id, result[0].ClientId);
    }

    private static DateTime RangeFrom(Order order) => order.CreatedAt.AddSeconds(-5);

    private static DateTime RangeTo(Order order) => order.CreatedAt.AddSeconds(5);

    private Order AddDeliveredOrder(params OrderItem[] items)
    {
        var order = AddPendingOrder(items);
        order.MarkAsReady();
        order.MarkAsOnTheWay();
        order.MarkAsDelivered();
        _context.SaveChanges();
        return order;
    }

    private Order AddPendingOrder(params OrderItem[] items)
    {
        var order = new Order(
            _user.Id,
            _deliveryType,
            new Address("Calle Principal", 123, null),
            [.. items]);
        _context.Orders.Add(order);
        _context.SaveChanges();
        return order;
    }
}

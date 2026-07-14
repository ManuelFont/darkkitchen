using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.Validators;
using DarkKitchen.Domain.ValueObjects;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class OrderRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IOrderRepository _repository = null!;
    private ProductCategory _category = null!;
    private Product _product = null!;
    private User _user = null!;
    private DeliveryType _deliveryType = null!;
    private Order _order = null!;

    [TestInitialize]
    public void Setup()
    {
        _category = new ProductCategory("Congelados", "Comida en heladera");
        _product = new Product("Hamburguesa", "Carne casera", 10m, _category, ["https://example.com/image.jpg"]);
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
        _context.Products.Add(_product);
        _context.Users.Add(_user);
        _context.DeliveryTypes.Add(_deliveryType);
        _context.SaveChanges();

        _repository = new OrderRepository(_context);
        _order = CreateOrder();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsOrder()
    {
        _repository.Add(_order);

        var found = _repository.GetById(_order.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(_order.Id, found.Id);
        Assert.AreEqual(_order.ClientId, found.ClientId);
        Assert.AreEqual(1, found.Items.Count);
    }

    [TestMethod]
    public void GetById_ReturnsOrderWithItemsAndProduct()
    {
        _repository.Add(_order);

        var found = _repository.GetById(_order.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(_order.Id, found.Id);
        Assert.AreEqual(_product.Id, found.Items[0].ProductId);
        Assert.AreEqual(_product.Name, found.Items[0].Product.Name);
        Assert.AreEqual(_order.DeliveryCost, found.DeliveryCost);
    }

    [TestMethod]
    public void GetAll_ReturnsAllOrders()
    {
        var secondOrder = new Order(
            _user.Id,
            _deliveryType,
            new Address("Otra Calle", 456, null),
            [new OrderItem(_product, 2)]);

        _repository.Add(_order);
        _repository.Add(secondOrder);

        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(8, result.Count());
    }

    [TestMethod]
    public void Exists_ReturnsTrueIfOrderExists()
    {
        _repository.Add(_order);

        var result = _repository.Exists(o => o.Id == _order.Id);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Update_UpdatesOrderStatus()
    {
        _repository.Add(_order);

        _order.MarkAsReady();
        _repository.Update(_order);

        var found = _repository.GetById(_order.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual(DarkKitchen.Domain.Enums.OrderStatus.Ready, found.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Update_ThrowsExceptionIfOrderDoesNotExist()
    {
        _repository.Update(_order);
    }

    [TestMethod]
    public void Delete_DeletesOrder()
    {
        _repository.Add(_order);

        _repository.Delete(_order.Id);

        var found = _repository.GetById(_order.Id);
        Assert.IsNull(found);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Delete_ThrowsExceptionIfOrderDoesNotExist()
    {
        _repository.Delete(_order.Id);
    }

    [TestMethod]
    public void GetByClient_ReturnsOnlyOrdersForThatClient()
    {
        var otherUser = new User("Jane", "Smith", "jane@test.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123457", new UruguayPhoneValidator()));
        _context.Users.Add(otherUser);
        _context.SaveChanges();

        var otherOrder = new Order(otherUser.Id, _deliveryType, new Address("Other St", 1, null), [new OrderItem(_product, 1)]);
        _repository.Add(_order);
        _repository.Add(otherOrder);

        var result = _repository.GetByClient(_user.Id, null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_order.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByClient_IncludesClientData()
    {
        _repository.Add(_order);

        var result = _repository.GetByClient(_user.Id, null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.IsNotNull(result[0].Client);
        Assert.AreEqual("John", result[0].Client.FirstName);
        Assert.AreEqual("Doe", result[0].Client.LastName);
        Assert.AreEqual("john@test.com", result[0].Client.Email);
    }

    [TestMethod]
    public void GetByClient_FiltersByStatus()
    {
        var pendingOrder = CreateOrder();
        var readyOrder = CreateOrder();
        _repository.Add(pendingOrder);
        _repository.Add(readyOrder);
        readyOrder.MarkAsReady();
        _repository.Update(readyOrder);

        var result = _repository.GetByClient(_user.Id, null, null, DarkKitchen.Domain.Enums.OrderStatus.Pending);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(pendingOrder.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByClient_FiltersByDateFrom()
    {
        _repository.Add(_order);
        var future = _order.CreatedAt.AddSeconds(1);

        var result = _repository.GetByClient(_user.Id, future, null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByClient_FiltersByDateTo()
    {
        _repository.Add(_order);
        var past = _order.CreatedAt.AddSeconds(-1);

        var result = _repository.GetByClient(_user.Id, null, past, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByFilters_ReturnsOrdersInDateRange()
    {
        _repository.Add(_order);
        var dateFrom = _order.CreatedAt.AddSeconds(-1);
        var dateTo = _order.CreatedAt.AddSeconds(1);

        var result = _repository.GetByFilters(dateFrom, dateTo, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(_order.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByFilters_ExcludesOrdersOutsideDateRange()
    {
        _repository.Add(_order);
        var dateFrom = _order.CreatedAt.AddSeconds(1);
        var dateTo = _order.CreatedAt.AddSeconds(2);

        var result = _repository.GetByFilters(dateFrom, dateTo, null, null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByFilters_FiltersByStreet()
    {
        var matchingOrder = new Order(_user.Id, _deliveryType, new Address("Main St", 1, null), [new OrderItem(_product, 1)]);
        var otherOrder = new Order(_user.Id, _deliveryType, new Address("Other Ave", 2, null), [new OrderItem(_product, 1)]);
        _repository.Add(matchingOrder);
        _repository.Add(otherOrder);

        var dateFrom = matchingOrder.CreatedAt.AddSeconds(-1);
        var dateTo = matchingOrder.CreatedAt.AddSeconds(1);

        var result = _repository.GetByFilters(dateFrom, dateTo, "Main", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(matchingOrder.Id, result[0].Id);
    }

    [TestMethod]
    public void GetByFilters_FiltersByStatus()
    {
        var pendingOrder = CreateOrder();
        var readyOrder = CreateOrder();
        _repository.Add(pendingOrder);
        _repository.Add(readyOrder);
        readyOrder.MarkAsReady();
        _repository.Update(readyOrder);

        var dateFrom = pendingOrder.CreatedAt.AddSeconds(-1);
        var dateTo = pendingOrder.CreatedAt.AddSeconds(1);

        var result = _repository.GetByFilters(dateFrom, dateTo, null, DarkKitchen.Domain.Enums.OrderStatus.Pending);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(pendingOrder.Id, result[0].Id);
    }

    private Order CreateOrder()
    {
        return new Order(
            _user.Id,
            _deliveryType,
            new Address("Calle Principal", 123, null),
            [new OrderItem(_product, 1)]);
    }
}

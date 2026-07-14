using DarkKitchen.Application.Services.Orders;
using DarkKitchen.Application.Services.Orders.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.ValueObjects;
using Moq;
using OrderStatus = DarkKitchen.Domain.Enums.OrderStatus;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class OrderServiceTests
{
    private Mock<IOrderRepository> _orderRepositoryMock = null!;
    private Mock<IProductRepository> _productRepositoryMock = null!;
    private Mock<IDeliveryTypeRepository> _deliveryTypeRepositoryMock = null!;
    private IOrderService _service = null!;
    private ProductCategory _category = null!;
    private DeliveryType _deliveryType = null!;

    [TestInitialize]
    public void SetUp()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _productRepositoryMock = new Mock<IProductRepository>();
        _deliveryTypeRepositoryMock = new Mock<IDeliveryTypeRepository>();
        _deliveryType = new DeliveryType("Envio express", 54m);
        _deliveryTypeRepositoryMock.Setup(r => r.GetById(_deliveryType.Id)).Returns(_deliveryType);
        _service = new OrderService(_orderRepositoryMock.Object, _productRepositoryMock.Object, _deliveryTypeRepositoryMock.Object);
        _category = new ProductCategory("Food", "Fresh meals");
    }

    [TestMethod]
    public void Create_WithValidExpressOrder_ReturnsOrderSummary()
    {
        var clientId = Guid.NewGuid();
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var promotion = new Promotion("Discount", 0.10m, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
        product.AddPromotion(promotion);

        var request = new CreateOrderDto
        {
            ClientId = clientId,
            DeliveryTypeId = _deliveryType.Id,
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items =
            [
                new CreateOrderItemDto
                {
                    ProductId = product.Id,
                    Quantity = 2
                }

            ]
        };

        Order? savedOrder = null;
        _productRepositoryMock.Setup(r => r.GetById(product.Id)).Returns(product);
        _orderRepositoryMock
            .Setup(r => r.Add(It.IsAny<Order>()))
            .Callback<Order>(order => savedOrder = order);

        var result = _service.Create(request);

        Assert.AreEqual(clientId, result.ClientId);
        Assert.AreEqual(54m, result.DeliveryCost);
        Assert.AreEqual(285.48m, result.Total);
        Assert.IsNotNull(savedOrder);
        Assert.AreEqual(OrderStatus.Pending, savedOrder.Status);
        _orderRepositoryMock.Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithEmptyItems_ThrowsInvalidArgumentException()
    {
        var request = new CreateOrderDto
        {
            ClientId = Guid.NewGuid(),
            DeliveryTypeId = _deliveryType.Id,
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items = []
        };

        _service.Create(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithNullItems_ThrowsInvalidArgumentException()
    {
        var request = new CreateOrderDto
        {
            ClientId = Guid.NewGuid(),
            DeliveryTypeId = _deliveryType.Id,
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items = null!
        };

        _service.Create(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithZeroQuantity_ThrowsInvalidArgumentException()
    {
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var request = new CreateOrderDto
        {
            ClientId = Guid.NewGuid(),
            DeliveryTypeId = _deliveryType.Id,
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items =
            [
                new CreateOrderItemDto
                {
                    ProductId = product.Id,
                    Quantity = 0
                }

            ]
        };

        _productRepositoryMock.Setup(r => r.GetById(product.Id)).Returns(product);

        _service.Create(request);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Create_WhenProductDoesNotExist_ThrowsResourceNotFoundException()
    {
        var missingProductId = Guid.NewGuid();
        var request = new CreateOrderDto
        {
            ClientId = Guid.NewGuid(),
            DeliveryTypeId = _deliveryType.Id,
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items =
            [
                new CreateOrderItemDto
                {
                    ProductId = missingProductId,
                    Quantity = 1
                }

            ]
        };

        _productRepositoryMock.Setup(r => r.GetById(missingProductId)).Returns((Product?)null);

        _service.Create(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithEmptyClientId_ThrowsInvalidArgumentException()
    {
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var request = new CreateOrderDto
        {
            ClientId = Guid.Empty,
            DeliveryTypeId = _deliveryType.Id,
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items =
            [
                new CreateOrderItemDto
                {
                    ProductId = product.Id,
                    Quantity = 1
                }

            ]
        };

        _productRepositoryMock.Setup(r => r.GetById(product.Id)).Returns(product);

        _service.Create(request);
    }

    [TestMethod]
    public void Create_WithTwentyFourHoursDelivery_Returns24hDeliveryCost()
    {
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var deliveryType = new DeliveryType("Envio 24 horas", 20m);
        var request = new CreateOrderDto
        {
            ClientId = Guid.NewGuid(),
            DeliveryTypeId = deliveryType.Id,
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items =
            [
                new CreateOrderItemDto
                {
                    ProductId = product.Id,
                    Quantity = 2
                }

            ]
        };

        _deliveryTypeRepositoryMock.Setup(r => r.GetById(deliveryType.Id)).Returns(deliveryType);
        _productRepositoryMock.Setup(r => r.GetById(product.Id)).Returns(product);

        var result = _service.Create(request);

        Assert.AreEqual(20m, result.DeliveryCost);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Create_WithUnsupportedDeliveryType_ThrowsResourceNotFoundException()
    {
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var request = new CreateOrderDto
        {
            ClientId = Guid.NewGuid(),
            DeliveryTypeId = Guid.NewGuid(),
            Street = "Main",
            DoorNumber = 123,
            Apartment = "A",
            Items =
            [
                new CreateOrderItemDto
                {
                    ProductId = product.Id,
                    Quantity = 1
                }

            ]
        };

        _productRepositoryMock.Setup(r => r.GetById(product.Id)).Returns(product);

        _service.Create(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void GetOrders_WhenDateFromIsAfterDateTo_ThrowsInvalidArgumentException()
    {
        var dto = new GetOrdersDto
        {
            DateFrom = new DateTime(2026, 12, 31),
            DateTo = new DateTime(2026, 1, 1)
        };

        _service.GetOrders(dto);
    }

    [TestMethod]
    public void GetOrders_WithValidDateRange_PassesFiltersToRepository()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 12, 31);
        var dto = new GetOrdersDto { DateFrom = dateFrom, DateTo = dateTo };

        _orderRepositoryMock
            .Setup(r => r.GetByFilters(dateFrom, dateTo, null, null))
            .Returns([]);

        _service.GetOrders(dto);

        _orderRepositoryMock.Verify(r => r.GetByFilters(dateFrom, dateTo, null, null), Times.Once);
    }

    [TestMethod]
    public void GetOrders_WithOptionalFilters_PassesFiltersToRepository()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 12, 31);
        var dto = new GetOrdersDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            Street = "Main Street",
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(r => r.GetByFilters(dateFrom, dateTo, "Main Street", OrderStatus.Pending))
            .Returns([]);

        _service.GetOrders(dto);

        _orderRepositoryMock.Verify(
            r => r.GetByFilters(dateFrom, dateTo, "Main Street", OrderStatus.Pending),
            Times.Once);
    }

    [TestMethod]
    public void GetOrders_WithNoDates_PassesNullFiltersToRepository()
    {
        var dto = new GetOrdersDto();

        _orderRepositoryMock
            .Setup(r => r.GetByFilters(null, null, null, null))
            .Returns([]);

        _service.GetOrders(dto);

        _orderRepositoryMock.Verify(r => r.GetByFilters(null, null, null, null), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void GetClientOrders_WithEmptyClientId_ThrowsInvalidArgumentException()
    {
        _service.GetClientOrders(Guid.Empty, new GetClientOrdersDto());
    }

    [TestMethod]
    public void GetClientOrders_WithNoFilters_PassesNullFiltersToRepository()
    {
        var clientId = Guid.NewGuid();
        var dto = new GetClientOrdersDto();

        _orderRepositoryMock
            .Setup(r => r.GetByClient(clientId, null, null, null))
            .Returns([]);

        _service.GetClientOrders(clientId, dto);

        _orderRepositoryMock.Verify(r => r.GetByClient(clientId, null, null, null), Times.Once);
    }

    [TestMethod]
    public void GetClientOrders_WithFilters_PassesFiltersToRepository()
    {
        var clientId = Guid.NewGuid();
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 12, 31);
        var dto = new GetClientOrdersDto
        {
            DateFrom = dateFrom,
            DateTo = dateTo,
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(r => r.GetByClient(clientId, dateFrom, dateTo, OrderStatus.Pending))
            .Returns([]);

        _service.GetClientOrders(clientId, dto);

        _orderRepositoryMock.Verify(
            r => r.GetByClient(clientId, dateFrom, dateTo, OrderStatus.Pending),
            Times.Once);
    }

    [TestMethod]
    public void GetOrders_WithOrders_ReturnsMappedItems()
    {
        var user = new User();
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var order = new Order(user.Id, _deliveryType, new Address("Main", 1, null), [new OrderItem(product, 2)]);
        typeof(Order).GetProperty(nameof(Order.Client))!.SetValue(order, user);

        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 12, 31);
        var dto = new GetOrdersDto { DateFrom = dateFrom, DateTo = dateTo };

        _orderRepositoryMock
            .Setup(r => r.GetByFilters(dateFrom, dateTo, null, null))
            .Returns([order]);

        var result = _service.GetOrders(dto);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(order.Id, result[0].OrderId);
        Assert.AreEqual(1, result[0].Items.Count);
        Assert.AreEqual("Burger", result[0].Items[0].ProductName);
        Assert.AreEqual(2, result[0].Items[0].Quantity);
        Assert.AreEqual(100m, result[0].Items[0].Price);
    }

    [TestMethod]
    public void GetClientOrders_WithOrders_ReturnsMappedSummaries()
    {
        var user = new User();
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var order = new Order(user.Id, _deliveryType, new Address("Main", 1, null), [new OrderItem(product, 2)]);
        typeof(Order).GetProperty(nameof(Order.Client))!.SetValue(order, user);

        var dto = new GetClientOrdersDto();

        _orderRepositoryMock
            .Setup(r => r.GetByClient(user.Id, null, null, null))
            .Returns([order]);

        var result = _service.GetClientOrders(user.Id, dto);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(order.Id, result[0].OrderId);
        Assert.AreEqual(OrderStatus.Pending, result[0].Status);
        Assert.AreEqual(2, result[0].ProductCount);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithInvalidStreet_ThrowsInvalidArgumentException()
    {
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var request = new CreateOrderDto
        {
            ClientId = Guid.NewGuid(),
            DeliveryTypeId = _deliveryType.Id,
            Street = " ",
            DoorNumber = 123,
            Apartment = "A",
            Items =
            [
                new CreateOrderItemDto
                {
                    ProductId = product.Id,
                    Quantity = 1
                }

            ]
        };

        _productRepositoryMock.Setup(r => r.GetById(product.Id)).Returns(product);

        _service.Create(request);
    }

    [TestMethod]
    public void GetById_WithExistingOrder_ReturnsMappedDetail()
    {
        var user = new User();
        var product = new Product("Burger", "Tasty meal", 100m, _category, ["https://example.com/image.jpg"]);
        var order = new Order(user.Id, _deliveryType, new Address("Main", 123, "A"), [new OrderItem(product, 2)]);
        typeof(Order).GetProperty(nameof(Order.Client))!.SetValue(order, user);

        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        var result = _service.GetById(order.Id);

        Assert.AreEqual(order.Id, result.OrderId);
        Assert.AreEqual(OrderStatus.Pending, result.Status);
        Assert.AreEqual(_deliveryType.Id, result.DeliveryTypeId);
        Assert.AreEqual(_deliveryType.Name, result.DeliveryTypeName);
        Assert.AreEqual("Main", result.Street);
        Assert.AreEqual(123, result.DoorNumber);
        Assert.AreEqual("A", result.Apartment);
        Assert.AreEqual(1, result.Items.Count);
        Assert.AreEqual("Burger", result.Items[0].ProductName);
        Assert.AreEqual(2, result.Items[0].Quantity);
        Assert.AreEqual(100m, result.Items[0].UnitPrice);
        Assert.AreEqual(200m, result.Items[0].Subtotal);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_WithNonExistingOrder_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();

        _orderRepositoryMock.Setup(r => r.GetById(id)).Returns((Order?)null);

        _service.GetById(id);
    }

    [TestMethod]
    public void MarkOrderAsReady_WithPendingOrder_ReturnsReadyStatus()
    {
        var order = BuildOrder();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        var result = _service.MarkOrderAsReady(order.Id);

        Assert.AreEqual(order.Id, result.OrderId);
        Assert.AreEqual(OrderStatus.Ready, result.Status);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void MarkOrderAsReady_WithNonExistingOrder_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(id)).Returns((Order?)null);

        _service.MarkOrderAsReady(id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkOrderAsReady_WithNonPendingOrder_ThrowsInvalidArgumentException()
    {
        var order = BuildOrder();
        order.MarkAsCancelled();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        _service.MarkOrderAsReady(order.Id);
    }

    [TestMethod]
    public void CancelOrder_WithPendingOrder_ReturnsCancelledStatus()
    {
        var order = BuildOrder();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        var result = _service.CancelOrder(order.Id);

        Assert.AreEqual(order.Id, result.OrderId);
        Assert.AreEqual(OrderStatus.Cancelled, result.Status);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void CancelOrder_WithNonExistingOrder_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(id)).Returns((Order?)null);

        _service.CancelOrder(id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CancelOrder_WithNonPendingOrder_ThrowsInvalidArgumentException()
    {
        var order = BuildOrder();
        order.MarkAsReady();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        _service.CancelOrder(order.Id);
    }

    [TestMethod]
    public void MarkOrderAsDelayed_WithPendingOrder_ReturnsDelayedStatus()
    {
        var order = BuildOrder();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        var result = _service.MarkOrderAsDelayed(order.Id);

        Assert.AreEqual(order.Id, result.OrderId);
        Assert.AreEqual(OrderStatus.Delayed, result.Status);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void MarkOrderAsDelayed_WithNonExistingOrder_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(id)).Returns((Order?)null);

        _service.MarkOrderAsDelayed(id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkOrderAsDelayed_WithNonPendingOrder_ThrowsInvalidArgumentException()
    {
        var order = BuildOrder();
        order.MarkAsReady();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        _service.MarkOrderAsDelayed(order.Id);
    }

    [TestMethod]
    public void MarkOrderAsOnTheWay_WithReadyOrder_ReturnsOnTheWayStatus()
    {
        var order = BuildOrder();
        order.MarkAsReady();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        var result = _service.MarkOrderAsOnTheWay(order.Id);

        Assert.AreEqual(order.Id, result.OrderId);
        Assert.AreEqual(OrderStatus.OnTheWay, result.Status);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void MarkOrderAsOnTheWay_WithNonExistingOrder_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(id)).Returns((Order?)null);

        _service.MarkOrderAsOnTheWay(id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkOrderAsOnTheWay_WithNonReadyOrder_ThrowsInvalidArgumentException()
    {
        var order = BuildOrder();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        _service.MarkOrderAsOnTheWay(order.Id);
    }

    [TestMethod]
    public void DeliverOrder_WithOnTheWayOrder_ReturnsDeliveredStatus()
    {
        var order = BuildOrder();
        order.MarkAsReady();
        order.MarkAsOnTheWay();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        var result = _service.DeliverOrder(order.Id);

        Assert.AreEqual(order.Id, result.OrderId);
        Assert.AreEqual(OrderStatus.Delivered, result.Status);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void DeliverOrder_WithNonExistingOrder_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(id)).Returns((Order?)null);

        _service.DeliverOrder(id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void DeliverOrder_WithNonOnTheWayOrder_ThrowsInvalidArgumentException()
    {
        var order = BuildOrder();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        _service.DeliverOrder(order.Id);
    }

    [TestMethod]
    public void MarkOrderAsNotDelivered_WithOnTheWayOrder_ReturnsNotDeliveredStatus()
    {
        var order = BuildOrder();
        order.MarkAsReady();
        order.MarkAsOnTheWay();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        var result = _service.MarkOrderAsNotDelivered(order.Id);

        Assert.AreEqual(order.Id, result.OrderId);
        Assert.AreEqual(OrderStatus.NotDelivered, result.Status);
        _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void MarkOrderAsNotDelivered_WithNonExistingOrder_ThrowsResourceNotFoundException()
    {
        var id = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetById(id)).Returns((Order?)null);

        _service.MarkOrderAsNotDelivered(id);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkOrderAsNotDelivered_WithNonOnTheWayOrder_ThrowsInvalidArgumentException()
    {
        var order = BuildOrder();
        _orderRepositoryMock.Setup(r => r.GetById(order.Id)).Returns(order);

        _service.MarkOrderAsNotDelivered(order.Id);
    }

    private Order BuildOrder() =>
        new(Guid.NewGuid(), _deliveryType, new Address("Main St", 123, null),
            [new OrderItem(new Product("Burger", "Tasty", 100m, _category, ["https://example.com/image.jpg"]), 1)]);
}

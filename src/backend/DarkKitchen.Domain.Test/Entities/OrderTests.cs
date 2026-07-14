using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public class OrderTests
{
    private Guid _validClientId = Guid.Empty;
    private Address _validAddress = null!;
    private DeliveryType _validDeliveryType = null!;
    private OrderItem _orderItem1 = null!;
    private OrderItem _orderItem2 = null!;
    private List<OrderItem> _validItems = null!;
    private ProductCategory _validCategory = null!;

    [TestInitialize]
    public void Initialize()
    {
        _validCategory = new ProductCategory("HotDrinks", "Perfect for winter");
        _validClientId = Guid.NewGuid();
        _validAddress = new Address("Main St", 123, null);
        _validDeliveryType = new DeliveryType("Envio express", 250m);
        _orderItem1 = new OrderItem(new Product("productName", "description", 3.5m, _validCategory, ["https://example.com/image.jpg"]), 1);
        _orderItem2 = new OrderItem(new Product("productName", "description", 1.2m, _validCategory, ["https://example.com/image.jpg"]), 1);
        _validItems = [_orderItem1, _orderItem2];
    }

    [TestMethod]
    public void CreateOrder_WithValidData_ShouldSetPropertiesCorrectly()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);

        Assert.AreEqual(_validClientId, order.ClientId);
        Assert.AreEqual(_validAddress, order.Address);
        Assert.AreEqual(OrderStatus.Pending, order.Status);
        Assert.AreNotEqual(Guid.Empty, order.Id);
    }

    [TestMethod]
    public void CreateOrder_WithValidData_ShouldSetCreatedAt()
    {
        var before = DateTime.UtcNow;
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        var after = DateTime.UtcNow;

        Assert.IsTrue(order.CreatedAt >= before);
        Assert.IsTrue(order.CreatedAt <= after);
    }

    [TestMethod]
    public void CreateOrder_WithInvalidUserId_ShouldThrowException()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _ = new Order(Guid.Empty, _validDeliveryType, _validAddress, _validItems));

        Assert.AreEqual("Client Id cannot be empty", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_WithInvalidDeliveryType_ShouldThrowException()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _ = new Order(_validClientId, null!, _validAddress, _validItems));

        Assert.AreEqual("Delivery type cannot be null", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_WithInvalidAddress_ShouldThrowException()
    {
        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _ = new Order(_validClientId, _validDeliveryType, null!, _validItems));

        Assert.AreEqual("Address cannot be null", ex.Message);
    }

    [TestMethod]
    public void CreateOrder_WithInvalidItems_ShouldThrowException()
    {
        var invalidItems = new List<OrderItem>();

        var ex = Assert.ThrowsException<ArgumentException>(() =>
            _ = new Order(_validClientId, _validDeliveryType, _validAddress, invalidItems));

        Assert.AreEqual("Items cannot be empty", ex.Message);
    }

    [TestMethod]
    public void MarkAsReady_WhenPending_ShouldChangeStatusToReady()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();

        Assert.AreEqual(OrderStatus.Ready, order.Status);
    }

    [TestMethod]
    public void MarkAsReady_WhenNotPending_ShouldThrowException()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsCancelled();

        var ex = Assert.ThrowsException<InvalidArgumentException>(order.MarkAsReady);
        Assert.AreEqual("Order can only be marked as ready if it is pending or delayed", ex.Message);
    }

    [TestMethod]
    public void Cancel_WhenPending_ShouldChangeStatusToCancelled()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsCancelled();

        Assert.AreEqual(OrderStatus.Cancelled, order.Status);
    }

    [TestMethod]
    public void Cancel_WhenNotPending_ShouldThrowException()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();

        var ex = Assert.ThrowsException<InvalidArgumentException>(order.MarkAsCancelled);
        Assert.AreEqual("Order can only be marked as cancelled if it is pending or delayed", ex.Message);
    }

    [TestMethod]
    public void MarkAsOnTheWay_WhenReady_ShouldChangeStatusToOnTheWay()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();
        order.MarkAsOnTheWay();

        Assert.AreEqual(OrderStatus.OnTheWay, order.Status);
    }

    [TestMethod]
    public void MarkAsOnTheWay_WhenNotReady_ShouldThrowException()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);

        var ex = Assert.ThrowsException<InvalidArgumentException>(order.MarkAsOnTheWay);
        Assert.AreEqual("Order can only be marked as on the way if it is ready", ex.Message);
    }

    [TestMethod]
    public void MarkAsDelivered_WhenOnTheWay_ShouldChangeStatusToDelivered()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();
        order.MarkAsOnTheWay();
        order.MarkAsDelivered();

        Assert.AreEqual(OrderStatus.Delivered, order.Status);
    }

    [TestMethod]
    public void MarkAsDelivered_WhenNotOnTheWay_ShouldThrowException()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);

        var ex = Assert.ThrowsException<InvalidArgumentException>(order.MarkAsDelivered);
        Assert.AreEqual("Order can only be marked as delivered if it is on the way", ex.Message);
    }

    [TestMethod]
    public void MarkAsNotDelivered_WhenOnTheWay_ShouldChangeStatusToNotDelivered()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();
        order.MarkAsOnTheWay();
        order.MarkAsNotDelivered();

        Assert.AreEqual(OrderStatus.NotDelivered, order.Status);
    }

    [TestMethod]
    public void MarkAsNotDelivered_WhenNotOnTheWay_ShouldThrowException()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);

        var ex = Assert.ThrowsException<InvalidArgumentException>(order.MarkAsNotDelivered);
        Assert.AreEqual("Order can only be marked as not delivered if it is on the way", ex.Message);
    }

    [TestMethod]
    public void SubTotal_ShouldReturnCorrectValue()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        Assert.AreEqual(_orderItem1.Subtotal + _orderItem2.Subtotal, order.Subtotal);
    }

    [TestMethod]
    public void Total_ShouldReturnCorrectValue()
    {
        var iva = 0.22m;
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        Assert.AreEqual((_orderItem1.Total + _orderItem2.Total + order.DeliveryCost) * (1 + iva), order.Total);
    }

    [TestMethod]
    public void Total_WithDifferentDeliveryCost_ShouldReturnCorrectValue()
    {
        var iva = 0.22m;
        var deliveryType = new DeliveryType("Envio dia siguiente", 180m);
        var order = new Order(_validClientId, deliveryType, _validAddress, _validItems);
        Assert.AreEqual((_orderItem1.Total + _orderItem2.Total + order.DeliveryCost) * (1 + iva), order.Total);
    }

    [TestMethod]
    public void CreateOrder_ShouldSetStatusChangedAtToCreatedAt()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);

        Assert.AreEqual(order.CreatedAt, order.StatusChangedAt);
    }

    [TestMethod]
    public void MarkAsReady_ShouldUpdateStatusChangedAt()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        var before = DateTime.UtcNow;
        order.MarkAsReady();
        var after = DateTime.UtcNow;

        Assert.IsTrue(order.StatusChangedAt >= before);
        Assert.IsTrue(order.StatusChangedAt <= after);
    }

    [TestMethod]
    public void MarkAsCancelled_ShouldUpdateStatusChangedAt()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        var before = DateTime.UtcNow;
        order.MarkAsCancelled();
        var after = DateTime.UtcNow;

        Assert.IsTrue(order.StatusChangedAt >= before);
        Assert.IsTrue(order.StatusChangedAt <= after);
    }

    [TestMethod]
    public void MarkAsOnTheWay_ShouldUpdateStatusChangedAt()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();
        var before = DateTime.UtcNow;
        order.MarkAsOnTheWay();
        var after = DateTime.UtcNow;

        Assert.IsTrue(order.StatusChangedAt >= before);
        Assert.IsTrue(order.StatusChangedAt <= after);
    }

    [TestMethod]
    public void MarkAsDelivered_ShouldUpdateStatusChangedAt()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();
        order.MarkAsOnTheWay();
        var before = DateTime.UtcNow;
        order.MarkAsDelivered();
        var after = DateTime.UtcNow;

        Assert.IsTrue(order.StatusChangedAt >= before);
        Assert.IsTrue(order.StatusChangedAt <= after);
    }

    [TestMethod]
    public void MarkAsNotDelivered_ShouldUpdateStatusChangedAt()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();
        order.MarkAsOnTheWay();
        var before = DateTime.UtcNow;
        order.MarkAsNotDelivered();
        var after = DateTime.UtcNow;

        Assert.IsTrue(order.StatusChangedAt >= before);
        Assert.IsTrue(order.StatusChangedAt <= after);
    }

    [TestMethod]
    public void MarkAsDelayed_WhenPending_ShouldChangeStatusToDelayed()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsDelayed();

        Assert.AreEqual(OrderStatus.Delayed, order.Status);
    }

    [TestMethod]
    public void MarkAsDelayed_WhenNotPending_ShouldThrowException()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsReady();

        var ex = Assert.ThrowsException<InvalidArgumentException>(order.MarkAsDelayed);
        Assert.AreEqual("Order can only be marked as delayed if it is pending", ex.Message);
    }

    [TestMethod]
    public void MarkAsDelayed_WhenAlreadyDelayed_ShouldThrowException()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsDelayed();

        var ex = Assert.ThrowsException<InvalidArgumentException>(order.MarkAsDelayed);
        Assert.AreEqual("Order is already delayed", ex.Message);
    }

    [TestMethod]
    public void MarkAsReady_WhenDelayed_ShouldChangeStatusToReady()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsDelayed();
        order.MarkAsReady();

        Assert.AreEqual(OrderStatus.Ready, order.Status);
    }

    [TestMethod]
    public void Cancel_WhenDelayed_ShouldChangeStatusToCancelled()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        order.MarkAsDelayed();
        order.MarkAsCancelled();

        Assert.AreEqual(OrderStatus.Cancelled, order.Status);
    }

    [TestMethod]
    public void MarkAsDelayed_ShouldUpdateStatusChangedAt()
    {
        var order = new Order(_validClientId, _validDeliveryType, _validAddress, _validItems);
        var before = DateTime.UtcNow;
        order.MarkAsDelayed();
        var after = DateTime.UtcNow;

        Assert.IsTrue(order.StatusChangedAt >= before);
        Assert.IsTrue(order.StatusChangedAt <= after);
    }
}

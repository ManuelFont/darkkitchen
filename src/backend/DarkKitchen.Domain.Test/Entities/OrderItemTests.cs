using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public class OrderItemTests
{
    private static readonly int OneYearFromToday = DateTime.Today.AddYears(1).Year;
    private readonly DateTime _validStart = new(OneYearFromToday, 1, 1);
    private readonly DateTime _validEnd = new(OneYearFromToday, 12, 31);
    private Product _product = null!;
    private Promotion _promotion = null!;
    private ProductCategory _validCategory = null!;

    [TestInitialize]
    public void Initialize()
    {
        _validCategory = new ProductCategory("HotDrinks", "Perfect for winter");
        _product = new Product("name", "description", 1, _validCategory, ["https://example.com/image.jpg"]);
        _promotion = new Promotion("name", 0.4m, _validStart, _validEnd);
    }

    [TestMethod]
    public void CreateOrderItem_WithValidData_ShouldSetPropertiesCorrectly()
    {
        var orderItem = new OrderItem(_product, 1);

        Assert.AreEqual(_product, orderItem.Product);
        Assert.AreEqual(1, orderItem.Quantity);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrderItem_WithNullProduct_ShouldThrowException()
    {
        _ = new OrderItem(null!, 1);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateOrderItem_WithInvalidQuantity_ShouldThrowException(int quantity)
    {
        _ = new OrderItem(
            new Product(
                "name",
                "description",
                1.3m,
                new ProductCategory("name", "des"),
                ["https://example.com/image.jpg"]),
            quantity);
    }

    [TestMethod]
    public void SubTotal_ShouldReturnCorrectValue()
    {
        var quantity = 4;
        var orderItem = new OrderItem(_product, quantity);
        Assert.AreEqual(_product.Price * quantity, orderItem.Subtotal);
    }

    [TestMethod]
    public void Total_WithoutProductPromotion_ShouldReturnCorrectValue()
    {
        var quantity = 4;
        var orderItem = new OrderItem(_product, quantity);
        Assert.AreEqual(_product.Price * quantity, orderItem.Total);
    }

    [TestMethod]
    public void Total_WithProductPromotion_ShouldReturnCorrectValue()
    {
        var quantity = 4;
        _product.AddPromotion(_promotion);
        var orderItem = new OrderItem(_product, quantity);
        Assert.AreEqual(_product.DiscountedPrice() * quantity, orderItem.Total);
    }
}

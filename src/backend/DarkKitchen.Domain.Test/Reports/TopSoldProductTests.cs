using DarkKitchen.Domain.Reports;

namespace DarkKitchen.Domain.Test.Reports;

[TestClass]
public class TopSoldProductTests
{
    [TestMethod]
    public void Constructor_WithValidData_SetsPropertiesCorrectly()
    {
        var productId = Guid.NewGuid();

        var topSoldProduct = new TopSoldProduct(productId, "Classic Burger", 42);

        Assert.AreEqual(productId, topSoldProduct.ProductId);
        Assert.AreEqual("Classic Burger", topSoldProduct.ProductName);
        Assert.AreEqual(42, topSoldProduct.QuantitySold);
    }

    [TestMethod]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        var productId = Guid.NewGuid();
        var first = new TopSoldProduct(productId, "Margherita", 10);
        var second = new TopSoldProduct(productId, "Margherita", 10);

        Assert.AreEqual(first, second);
    }
}

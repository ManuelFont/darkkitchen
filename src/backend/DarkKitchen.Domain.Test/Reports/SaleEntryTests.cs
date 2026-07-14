using DarkKitchen.Domain.Reports;

namespace DarkKitchen.Domain.Test.Reports;

[TestClass]
public class SaleEntryTests
{
    [TestMethod]
    public void Constructor_WithValidData_SetsPropertiesCorrectly()
    {
        var clientId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 15);

        var saleEntry = new SaleEntry(createdAt, clientId, "Juan Perez", 3500m, 600m, 5002m);

        Assert.AreEqual(createdAt, saleEntry.CreatedAt);
        Assert.AreEqual(clientId, saleEntry.ClientId);
        Assert.AreEqual("Juan Perez", saleEntry.ClientName);
        Assert.AreEqual(3500m, saleEntry.ItemsTotal);
        Assert.AreEqual(600m, saleEntry.DeliveryCost);
        Assert.AreEqual(5002m, saleEntry.Total);
    }

    [TestMethod]
    public void Equals_WithSameValues_ReturnsTrue()
    {
        var clientId = Guid.NewGuid();
        var createdAt = new DateTime(2026, 1, 15);
        var first = new SaleEntry(createdAt, clientId, "Juan Perez", 100m, 10m, 134.2m);
        var second = new SaleEntry(createdAt, clientId, "Juan Perez", 100m, 10m, 134.2m);

        Assert.AreEqual(first, second);
    }
}

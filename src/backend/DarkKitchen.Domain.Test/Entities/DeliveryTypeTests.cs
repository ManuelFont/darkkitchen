using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public sealed class DeliveryTypeTests
{
    [TestMethod]
    public void Create_WithValidData_ShouldSetProperties()
    {
        var deliveryType = new DeliveryType("Envio express", 250m);

        Assert.AreNotEqual(Guid.Empty, deliveryType.Id);
        Assert.AreEqual("Envio express", deliveryType.Name);
        Assert.AreEqual(250m, deliveryType.Cost);
    }

    [TestMethod]
    public void Create_WithEmptyName_ShouldThrowException()
    {
        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            _ = new DeliveryType(" ", 250m));

        Assert.AreEqual("Delivery type name is required", ex.Message);
    }

    [TestMethod]
    public void Create_WithNegativeCost_ShouldThrowException()
    {
        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            _ = new DeliveryType("Envio express", -1m));

        Assert.AreEqual("Delivery type cost cannot be negative", ex.Message);
    }

    [TestMethod]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        var deliveryType = new DeliveryType("Envio express", 250m);

        deliveryType.Update("Envio en el dia", 200m);

        Assert.AreEqual("Envio en el dia", deliveryType.Name);
        Assert.AreEqual(200m, deliveryType.Cost);
    }
}

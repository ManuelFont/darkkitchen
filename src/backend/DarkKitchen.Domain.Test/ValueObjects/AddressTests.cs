using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Test.ValueObjects;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void CreateAddress_WithValidData_ShouldSetPropertiesCorrectly()
    {
        var address = new Address("street", 1, "apartment");

        Assert.AreEqual("street", address.Street);
        Assert.AreEqual("apartment", address.Apartment);
        Assert.AreEqual(1, address.DoorNumber);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("value.")]
    [DataRow("value$")]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateAddress_WithInvalidStreet_ShouldThrowException(string street)
    {
        _ = new Address(street, 123, null);
    }

    [TestMethod]
    [DataRow("Avenida 18 de Julio")]
    [DataRow("Calle 9")]
    public void CreateAddress_WithAlphanumericStreet_ShouldSetStreet(string street)
    {
        var address = new Address(street, 123, null);

        Assert.AreEqual(street, address.Street);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateAddress_WithInvalidDoorNumber_ShouldThrowException(int doorNumber)
    {
        _ = new Address("Main St", doorNumber, null);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("value.")]
    [DataRow("value$")]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateAddress_WithInvalidApartment_ShouldThrowException(string apartment)
    {
        _ = new Address("Main St", 123, apartment);
    }

    [TestMethod]
    [DataRow("2B")]
    [DataRow("apt 3")]
    [DataRow("101")]
    public void CreateAddress_WithAlphanumericApartment_ShouldSetApartment(string apartment)
    {
        var address = new Address("Main St", 123, apartment);

        Assert.AreEqual(apartment, address.Apartment);
    }
}

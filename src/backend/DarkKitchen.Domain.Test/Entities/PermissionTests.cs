using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public sealed class PermissionTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePermission_WithEmptyName_ThrowsArgumentException()
    {
        _ = new Permission(1, string.Empty);
    }

    [TestMethod]
    public void CreatePermission_UsingConstructor_SetsProperties()
    {
        var permission = new Permission(1, "CanCreateOrder");

        Assert.AreEqual(1, permission.PermissionId);
        Assert.AreEqual("CanCreateOrder", permission.PermissionName);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreatePermission_WithZeroId_ThrowsArgumentException()
    {
        _ = new Permission(0, "CanCreateOrder");
    }
}

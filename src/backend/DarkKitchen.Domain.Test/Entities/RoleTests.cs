using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public sealed class RoleTests
{
    [TestMethod]
    public void RolePermissions_WhenRoleIsCreated_StartsAsEmpty()
    {
        var role = new Role(1, "Administrator");

        Assert.IsNotNull(role.RolePermissions);
        Assert.AreEqual(0, role.RolePermissions.Count);
    }

    [TestMethod]
    public void CreateRole_UsingConstructor_SetsProperties()
    {
        var role = new Role(1, "Administrator");

        Assert.AreEqual(1, role.RoleId);
        Assert.AreEqual("Administrator", role.RoleName);
        Assert.AreEqual(0, role.RolePermissions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateRole_WithEmptyName_ThrowsArgumentException()
    {
        _ = new Role(1, string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateRole_WithZeroId_ThrowsArgumentException()
    {
        _ = new Role(0, "Administrator");
    }

    [TestMethod]
    public void AddPermission_WithValidPermission_AddsPermissionToRole()
    {
        var role = new Role(1, "Administrator");
        var permission = new Permission(1, "CanCreateOrder");

        role.AddPermission(permission);

        Assert.AreEqual(1, role.RolePermissions.Count);
        Assert.AreSame(permission, role.RolePermissions[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void AddPermission_WithNullPermission_ThrowsArgumentException()
    {
        var role = new Role(1, "Administrator");

        role.AddPermission(null!);
    }
}

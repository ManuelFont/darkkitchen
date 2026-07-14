using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public sealed class AuditLogTests
{
    private static AuditLog Build(
        AuditAction action = AuditAction.Created,
        string entityName = "Producto",
        string description = "Producto creado: Pizza",
        string userEmail = "admin@darkkitchen.com")
    {
        return new AuditLog(action, entityName, Guid.NewGuid(), description, Guid.NewGuid(), userEmail);
    }

    [TestMethod]
    public void Given_ValidData_When_Constructed_Then_SetsIdAndTimestamp()
    {
        var log = Build();

        Assert.AreNotEqual(Guid.Empty, log.Id);
        Assert.AreNotEqual(default, log.Timestamp);
    }

    [TestMethod]
    public void Given_ValidData_When_Constructed_Then_StoresAllValues()
    {
        var entityId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var log = new AuditLog(AuditAction.Updated, "Promocion", entityId, "Promoción modificada", userId, "admin@darkkitchen.com");

        Assert.AreEqual(AuditAction.Updated, log.Action);
        Assert.AreEqual("Promocion", log.EntityName);
        Assert.AreEqual(entityId, log.EntityId);
        Assert.AreEqual("Promoción modificada", log.Description);
        Assert.AreEqual(userId, log.UserId);
        Assert.AreEqual("admin@darkkitchen.com", log.UserEmail);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Given_EmptyEntityName_When_Constructed_Then_Throws()
    {
        Build(entityName: "   ");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Given_EmptyDescription_When_Constructed_Then_Throws()
    {
        Build(description: string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Given_EmptyEntityId_When_Constructed_Then_Throws()
    {
        new AuditLog(AuditAction.Created, "Producto", Guid.Empty, "desc", Guid.NewGuid(), "admin@darkkitchen.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Given_EmptyUserId_When_Constructed_Then_Throws()
    {
        new AuditLog(AuditAction.Created, "Producto", Guid.NewGuid(), "desc", Guid.Empty, "admin@darkkitchen.com");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Given_EmptyUserEmail_When_Constructed_Then_Throws()
    {
        Build(userEmail: string.Empty);
    }
}

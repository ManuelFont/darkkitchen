using DarkKitchen.Application.Services.Roles;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class RoleServiceTests
{
    private Mock<IRoleRepository> _roleRepositoryMock = null!;
    private Mock<IPermissionRepository> _permissionRepositoryMock = null!;
    private IRoleService _roleService = null!;

    [TestInitialize]
    public void Setup()
    {
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _permissionRepositoryMock = new Mock<IPermissionRepository>();
        _roleService = new RoleService(_roleRepositoryMock.Object, _permissionRepositoryMock.Object);
    }

    [TestMethod]
    public void AssignPermissionToRole_WithValidIds_AssignsPermission()
    {
        var role = new Role(1, "Administrator");
        var permission = new Permission(1, "CanCreateOrder");

        _roleRepositoryMock.Setup(r => r.GetById(1)).Returns(role);
        _permissionRepositoryMock.Setup(r => r.GetById(1)).Returns(permission);

        _roleService.AssignPermissionToRole(1, 1);

        Assert.AreEqual(1, role.RolePermissions.Count);
        _roleRepositoryMock.Verify(r => r.Update(role), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void AssignPermissionToRole_WithNonExistentRole_ThrowsResourceNotFoundException()
    {
        _roleRepositoryMock.Setup(r => r.GetById(999)).Returns((Role?)null);

        _roleService.AssignPermissionToRole(999, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void AssignPermissionToRole_WithNonExistentPermission_ThrowsResourceNotFoundException()
    {
        var role = new Role(1, "Administrator");
        _roleRepositoryMock.Setup(r => r.GetById(1)).Returns(role);
        _permissionRepositoryMock.Setup(r => r.GetById(999)).Returns((Permission?)null);

        _roleService.AssignPermissionToRole(1, 999);
    }

    [TestMethod]
    public void GetRoleById_WithExistingId_ReturnsRole()
    {
        var role = new Role(1, "Administrator");
        _roleRepositoryMock.Setup(r => r.GetById(1)).Returns(role);

        var result = _roleService.GetRoleById(1);

        Assert.AreEqual(1, result.RoleId);
        Assert.AreEqual("Administrator", result.RoleName);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetRoleById_WithNonExistentId_ThrowsResourceNotFoundException()
    {
        _roleRepositoryMock.Setup(r => r.GetById(999)).Returns((Role?)null);

        _roleService.GetRoleById(999);
    }
}

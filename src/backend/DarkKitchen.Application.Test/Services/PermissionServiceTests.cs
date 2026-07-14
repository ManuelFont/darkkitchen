using System.Linq.Expressions;
using DarkKitchen.Application.Services.Permissions;
using DarkKitchen.Application.Services.Permissions.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class PermissionServiceTests
{
    private Mock<IPermissionRepository> _permissionRepositoryMock = null!;
    private IPermissionService _permissionService = null!;

    [TestInitialize]
    public void Setup()
    {
        _permissionRepositoryMock = new Mock<IPermissionRepository>();
        _permissionService = new PermissionService(_permissionRepositoryMock.Object);
    }

    [TestMethod]
    public void CreatePermission_WithValidData_ReturnsPermissionId()
    {
        var dto = new CreatePermissionDto
        {
            PermissionId = 1,
            PermissionName = "CanCreateOrder"
        };

        _permissionRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Permission, bool>>>()))
            .Returns(false);

        var result = _permissionService.CreatePermission(dto);

        Assert.AreEqual(1, result);
        _permissionRepositoryMock.Verify(r => r.Add(It.IsAny<Permission>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void CreatePermission_WithDuplicateName_ThrowsDuplicateResourceException()
    {
        var dto = new CreatePermissionDto
        {
            PermissionId = 1,
            PermissionName = "CanCreateOrder"
        };

        _permissionRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<Permission, bool>>>()))
            .Returns(true);

        _permissionService.CreatePermission(dto);
    }
}

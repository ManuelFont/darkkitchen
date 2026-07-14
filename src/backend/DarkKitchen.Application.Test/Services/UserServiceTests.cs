using System.Linq.Expressions;
using DarkKitchen.Application.Services.Users;
using DarkKitchen.Application.Services.Users.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.Validators;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IRoleRepository> _roleRepositoryMock = null!;
    private IPhoneValidator _phoneValidator = null!;
    private IUserService _userService = null!;

    [TestInitialize]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _phoneValidator = new UruguayPhoneValidator();
        _userService = new UserService(_userRepositoryMock.Object, _roleRepositoryMock.Object, _phoneValidator);
    }

    [TestMethod]
    public void Register_WithValidData_CreatesNewUserAndReturnsNewUserId()
    {
        var dto = new RegisterUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "ValidPassword1!2$3%",
            Phone = "099123456"
        };

        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);

        var result = _userService.Register(dto);

        Assert.AreNotEqual(Guid.Empty, result);
        _userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Register_WithExistingEmail_ThrowsDuplicateResourceException()
    {
        var dto = new RegisterUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "ValidPassword1!2$3%",
            Phone = "099123456"
        };

        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        _userService.Register(dto);
    }

    [TestMethod]
    public void GetUsers_ReturnsAllMappedUsers()
    {
        var role = new Role(1, "Customer");
        var user = new User("John", "Doe", "john@example.com",
            DarkKitchen.Domain.ValueObjects.Password.Create("Abcdefghijk#1aaa"),
            DarkKitchen.Domain.ValueObjects.PhoneNumber.Create("099123456", _phoneValidator))
        {
            Role = role
        };

        _userRepositoryMock
            .Setup(r => r.GetByFilters(null, null))
            .Returns([user]);

        var result = _userService.GetUsers(new GetUsersDto());

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(user.Id, result[0].Id);
        Assert.AreEqual("John", result[0].FirstName);
        Assert.AreEqual("Customer", result[0].RoleName);
    }

    [TestMethod]
    public void GetUsers_PassesFiltersToRepository()
    {
        _userRepositoryMock
            .Setup(r => r.GetByFilters("John", "Customer"))
            .Returns([]);

        _userService.GetUsers(new GetUsersDto { Search = "John", Role = "Customer" });

        _userRepositoryMock.Verify(r => r.GetByFilters("John", "Customer"), Times.Once);
    }

    [TestMethod]
    public void CreateByAdmin_WithValidData_CreatesUserWithSpecifiedRole()
    {
        var role = new Role(2, "Dispatcher");
        var dto = new CreateAdminUserDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "ValidPassword1!2$3%",
            Phone = "099123456",
            RoleId = 2
        };

        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);
        _roleRepositoryMock.Setup(r => r.GetById(2)).Returns(role);

        var result = _userService.CreateByAdmin(dto);

        Assert.AreEqual("Jane", result.FirstName);
        Assert.AreEqual(2, result.RoleId);
        Assert.AreEqual("Dispatcher", result.RoleName);
        _userRepositoryMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void CreateByAdmin_WithExistingEmail_ThrowsDuplicateResourceException()
    {
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        _userService.CreateByAdmin(new CreateAdminUserDto { Email = "jane@example.com", RoleId = 1 });
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void CreateByAdmin_WithNonExistingRole_ThrowsResourceNotFoundException()
    {
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);
        _roleRepositoryMock.Setup(r => r.GetById(99)).Returns((Role?)null);

        _userService.CreateByAdmin(new CreateAdminUserDto { Email = "jane@example.com", RoleId = 99 });
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Update_WhenRequesterIsTarget_ThrowsInvalidArgumentException()
    {
        var id = Guid.NewGuid();

        _userService.Update(id, id, new UpdateUserDto());
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_WhenUserDoesNotExist_ThrowsResourceNotFoundException()
    {
        _userRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).Returns((User?)null);

        _userService.Update(Guid.NewGuid(), Guid.NewGuid(), new UpdateUserDto());
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_WhenRoleDoesNotExist_ThrowsResourceNotFoundException()
    {
        var user = new User();
        _userRepositoryMock.Setup(r => r.GetById(user.Id)).Returns(user);
        _roleRepositoryMock.Setup(r => r.GetById(99)).Returns((Role?)null);

        _userService.Update(Guid.NewGuid(), user.Id, new UpdateUserDto { RoleId = 99 });
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Update_WhenEmailTakenByAnotherUser_ThrowsDuplicateResourceException()
    {
        var user = new User();
        var role = new Role(1, "Customer");
        _userRepositoryMock.Setup(r => r.GetById(user.Id)).Returns(user);
        _roleRepositoryMock.Setup(r => r.GetById(1)).Returns(role);
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        _userService.Update(Guid.NewGuid(), user.Id, new UpdateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "taken@example.com",
            Phone = "099123456",
            RoleId = 1
        });
    }

    [TestMethod]
    public void Update_WithValidData_UpdatesUserAndReturnsSummary()
    {
        var user = new User();
        var role = new Role(2, "Dispatcher");
        _userRepositoryMock.Setup(r => r.GetById(user.Id)).Returns(user);
        _roleRepositoryMock.Setup(r => r.GetById(2)).Returns(role);
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);

        var result = _userService.Update(Guid.NewGuid(), user.Id, new UpdateUserDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Phone = "099123456",
            RoleId = 2
        });

        Assert.AreEqual("Jane", result.FirstName);
        Assert.AreEqual("Smith", result.LastName);
        Assert.AreEqual(2, result.RoleId);
        Assert.AreEqual("Dispatcher", result.RoleName);
        _userRepositoryMock.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Delete_WhenRequesterIsTarget_ThrowsInvalidArgumentException()
    {
        var id = Guid.NewGuid();

        _userService.Delete(id, id);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Delete_WhenUserDoesNotExist_ThrowsResourceNotFoundException()
    {
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);

        _userService.Delete(Guid.NewGuid(), Guid.NewGuid());
    }

    [TestMethod]
    public void Delete_WithValidData_CallsRepositoryDelete()
    {
        var targetId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        _userService.Delete(Guid.NewGuid(), targetId);

        _userRepositoryMock.Verify(r => r.Delete(targetId), Times.Once);
    }
}

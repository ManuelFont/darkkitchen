using System.Linq.Expressions;
using DarkKitchen.Application.Services.Sessions;
using DarkKitchen.Application.Services.Sessions.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.Repositories.Sessions;
using DarkKitchen.Domain.ValueObjects;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class SessionServiceTests
{
    private Mock<ISessionRepository> _sessionRepositoryMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private SessionService _sessionService = null!;

    [TestInitialize]
    public void Setup()
    {
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _sessionService = new SessionService(
            _sessionRepositoryMock.Object,
            _userRepositoryMock.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetUserByToken_WithNonExistentToken_ThrowsResourceNotFoundException()
    {
        var token = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetById(token))
            .Returns((Session?)null);

        _sessionService.GetUserByToken(token);
    }

    [TestMethod]
    [ExpectedException(typeof(TokenExpiredException))]
    public void GetUserByToken_WithExpiredToken_ThrowsTokenExpiredException()
    {
        var token = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var expiredSession = new Session(userId, DateTime.Now.AddHours(2));

        _sessionRepositoryMock
            .Setup(r => r.GetById(token))
            .Returns(expiredSession);

        typeof(Session)
            .GetProperty(nameof(Session.ExpiresAt))!
            .SetValue(expiredSession, DateTime.Now.AddHours(-1));

        _sessionService.GetUserByToken(token);
    }

    [TestMethod]
    public void GetUserByToken_WithValidToken_ReturnsUser()
    {
        var token = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var session = new Session(userId, DateTime.Now.AddHours(2));
        var user = new User();

        _sessionRepositoryMock
            .Setup(r => r.GetById(token))
            .Returns(session);

        _userRepositoryMock
            .Setup(r => r.GetById(userId))
            .Returns(user);

        var result = _sessionService.GetUserByToken(token);

        Assert.AreEqual(user, result);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Login_WithNonExistentEmail_ThrowsInvalidArgumentException()
    {
        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(false);

        _sessionService.Login(new LoginDto { Email = "unknown@example.com", Password = "ValidPassword1!2$3%" });
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Login_WithWrongPassword_ThrowsInvalidArgumentException()
    {
        var password = Password.Create("ValidPassword1!2$3%");
        var user = new User("John", "Doe", "john@example.com", password,
            PhoneNumber.Create("099123456", new DarkKitchen.Domain.Validators.UruguayPhoneValidator()));

        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        _userRepositoryMock
            .Setup(r => r.GetAll())
            .Returns([user]);

        _sessionService.Login(new LoginDto { Email = "john@example.com", Password = "WrongPassword1!2$3%" });
    }

    [TestMethod]
    public void Login_WithValidCredentials_ReturnsTokenAndRole()
    {
        var password = Password.Create("ValidPassword1!2$3%");
        var user = new User("John", "Doe", "john@example.com", password,
            PhoneNumber.Create("099123456", new DarkKitchen.Domain.Validators.UruguayPhoneValidator()))
        {
            Role = new Role(1, "Customer")
        };

        _userRepositoryMock
            .Setup(r => r.Exists(It.IsAny<Expression<Func<User, bool>>>()))
            .Returns(true);

        _userRepositoryMock
            .Setup(r => r.GetAll())
            .Returns([user]);

        _userRepositoryMock
            .Setup(r => r.GetById(user.Id))
            .Returns(user);

        var result = _sessionService.Login(new LoginDto { Email = "john@example.com", Password = "ValidPassword1!2$3%" });

        Assert.AreNotEqual(Guid.Empty, result.Token);
        Assert.AreEqual("Customer", result.RoleName);
        _sessionRepositoryMock.Verify(r => r.Add(It.IsAny<Session>()), Times.Once);
    }

    [TestMethod]
    public void Logout_WithValidToken_CallsDelete()
    {
        var token = Guid.NewGuid();
        var session = new Session(Guid.NewGuid(), DateTime.Now.AddHours(2));

        _sessionRepositoryMock
            .Setup(r => r.GetById(token))
            .Returns(session);

        _sessionService.Logout(token);

        _sessionRepositoryMock.Verify(r => r.Delete(token), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Logout_WithNonExistentToken_ThrowsResourceNotFoundException()
    {
        var token = Guid.NewGuid();

        _sessionRepositoryMock
            .Setup(r => r.GetById(token))
            .Returns((Session?)null);

        _sessionService.Logout(token);
    }
}

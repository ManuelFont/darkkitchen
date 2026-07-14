using DarkKitchen.Application.Services.Sessions;
using DarkKitchen.Application.Services.Sessions.Dtos;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.Sessions;
using DarkKitchen.WebApi.Requests.Sessions;
using DarkKitchen.WebApi.Responses.Sessions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class SessionControllerTests
{
    private Mock<ISessionService> _sessionServiceMock = null!;
    private SessionController _sessionController = null!;

    [TestInitialize]
    public void Setup()
    {
        _sessionServiceMock = new Mock<ISessionService>();
        _sessionController = new SessionController(_sessionServiceMock.Object);
    }

    [TestMethod]
    public void Login_WithValidCredentials_Returns201WithTokenAndRole()
    {
        var expectedToken = Guid.NewGuid();
        var request = new LoginRequest
        {
            Email = "john@example.com",
            Password = "ValidPassword1!2$3%"
        };

        _sessionServiceMock
            .Setup(s => s.Login(It.IsAny<LoginDto>()))
            .Returns(new LoginResultDto { Token = expectedToken, RoleName = "Administrator" });

        var result = (CreatedResult)_sessionController.Login(request);

        Assert.AreEqual(201, result.StatusCode);
        var response = (LoginResponse)result.Value!;
        Assert.AreEqual(expectedToken, response.Token);
        Assert.AreEqual("Administrator", response.Role);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Login_WithInvalidEmail_PropagatesException()
    {
        var request = new LoginRequest
        {
            Email = "unknown@example.com",
            Password = "ValidPassword1!2$3%"
        };

        _sessionServiceMock
            .Setup(s => s.Login(It.IsAny<LoginDto>()))
            .Throws(new InvalidArgumentException("Invalid email or password"));

        _sessionController.Login(request);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Login_WithWrongPassword_PropagatesException()
    {
        var request = new LoginRequest
        {
            Email = "john@example.com",
            Password = "WrongPassword1!2$3%"
        };

        _sessionServiceMock
            .Setup(s => s.Login(It.IsAny<LoginDto>()))
            .Throws(new InvalidArgumentException("Invalid email or password"));

        _sessionController.Login(request);
    }

    [TestMethod]
    public void Logout_WithValidToken_Returns204()
    {
        var token = Guid.NewGuid();

        var result = (NoContentResult)_sessionController.Logout(token);

        Assert.AreEqual(204, result.StatusCode);
        _sessionServiceMock.Verify(s => s.Logout(token), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Logout_WithNonExistentToken_PropagatesException()
    {
        var token = Guid.NewGuid();

        _sessionServiceMock
            .Setup(s => s.Logout(token))
            .Throws(new ResourceNotFoundException("Session", token));

        _sessionController.Logout(token);
    }
}

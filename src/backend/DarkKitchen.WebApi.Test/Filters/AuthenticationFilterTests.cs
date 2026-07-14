using DarkKitchen.Application.Services.Sessions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DarkKitchen.WebApi.Test.Filters;

[TestClass]
public sealed class AuthenticationFilterTests
{
    private Mock<ISessionService> _sessionServiceMock = null!;
    private AuthenticationFilter _filter = null!;

    [TestInitialize]
    public void Setup()
    {
        _sessionServiceMock = new Mock<ISessionService>();
        _filter = new AuthenticationFilter(_sessionServiceMock.Object);
    }

    private static AuthorizationFilterContext BuildContext(string? authorizationHeader = null)
    {
        var httpContext = new DefaultHttpContext();
        if(authorizationHeader is not null)
        {
            httpContext.Request.Headers["Authorization"] = authorizationHeader;
        }

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new AuthorizationFilterContext(actionContext, []);
    }

    [TestMethod]
    public void Given_MissingAuthorizationHeader_When_OnAuthorization_Then_Returns401()
    {
        var context = BuildContext();

        _filter.OnAuthorization(context);

        Assert.AreEqual(401, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_MissingAuthorizationHeader_When_OnAuthorization_Then_ReturnsNotAuthenticated()
    {
        var context = BuildContext();

        _filter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("UnAuthorized", body.InnerCode);
        Assert.AreEqual("Not authenticated", body.Message);
    }

    [TestMethod]
    public void Given_EmptyAuthorizationHeader_When_OnAuthorization_Then_Returns401()
    {
        var context = BuildContext("   ");

        _filter.OnAuthorization(context);

        Assert.AreEqual(401, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_InvalidTokenFormat_When_OnAuthorization_Then_Returns401()
    {
        var context = BuildContext("not-a-guid");

        _filter.OnAuthorization(context);

        Assert.AreEqual(401, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_InvalidTokenFormat_When_OnAuthorization_Then_ReturnsTokenInvalid()
    {
        var context = BuildContext("not-a-guid");

        _filter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("UnAuthorized", body.InnerCode);
        Assert.AreEqual("Token invalid", body.Message);
    }

    [TestMethod]
    public void Given_ValidTokenAndUserExists_When_OnAuthorization_Then_DoesNotSetResult()
    {
        var token = Guid.NewGuid();

        _sessionServiceMock
            .Setup(s => s.GetUserByToken(token))
            .Returns(new User());

        var context = BuildContext(token.ToString());

        _filter.OnAuthorization(context);

        Assert.IsNull(context.Result);
    }

    [TestMethod]
    public void Given_ValidTokenAndUserExists_When_OnAuthorization_Then_ServiceIsCalledWithToken()
    {
        var token = Guid.NewGuid();

        _sessionServiceMock
            .Setup(s => s.GetUserByToken(token))
            .Returns(new User());

        var context = BuildContext(token.ToString());

        _filter.OnAuthorization(context);

        _sessionServiceMock.Verify(s => s.GetUserByToken(token), Times.Once);
    }

    [TestMethod]
    public void Given_ExpiredToken_When_OnAuthorization_Then_Returns401()
    {
        var token = Guid.NewGuid();

        _sessionServiceMock
            .Setup(s => s.GetUserByToken(token))
            .Throws(new TokenExpiredException("Token expired"));

        var context = BuildContext(token.ToString());

        _filter.OnAuthorization(context);

        Assert.AreEqual(401, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_ExpiredToken_When_OnAuthorization_Then_ReturnsTokenExpiredMessage()
    {
        var token = Guid.NewGuid();

        _sessionServiceMock
            .Setup(s => s.GetUserByToken(token))
            .Throws(new TokenExpiredException("Token expired"));

        var context = BuildContext(token.ToString());

        _filter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("UnAuthorized", body.InnerCode);
        Assert.AreEqual("Token expired", body.Message);
    }

    [TestMethod]
    public void Given_NonExistentToken_When_OnAuthorization_Then_Returns401()
    {
        var token = Guid.NewGuid();

        _sessionServiceMock
            .Setup(s => s.GetUserByToken(token))
            .Throws(new Exception("not found"));

        var context = BuildContext(token.ToString());

        _filter.OnAuthorization(context);

        Assert.AreEqual(401, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_NonExistentToken_When_OnAuthorization_Then_ReturnsTokenDoesNotExist()
    {
        var token = Guid.NewGuid();

        _sessionServiceMock
            .Setup(s => s.GetUserByToken(token))
            .Throws(new Exception("not found"));

        var context = BuildContext(token.ToString());

        _filter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("UnAuthorized", body.InnerCode);
        Assert.AreEqual("Token does not exist", body.Message);
    }

    [TestMethod]
    public void Given_MissingHeader_When_OnAuthorization_Then_ServiceIsNotCalled()
    {
        var context = BuildContext();

        _filter.OnAuthorization(context);

        _sessionServiceMock.Verify(s => s.GetUserByToken(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void Given_InvalidTokenFormat_When_OnAuthorization_Then_ServiceIsNotCalled()
    {
        var context = BuildContext("not-a-guid");

        _filter.OnAuthorization(context);

        _sessionServiceMock.Verify(s => s.GetUserByToken(It.IsAny<Guid>()), Times.Never);
    }

    [TestMethod]
    public void Given_ValidTokenAndUserExists_When_OnAuthorization_Then_StoresUserInHttpContextItems()
    {
        var token = Guid.NewGuid();
        var user = new User();

        _sessionServiceMock
            .Setup(s => s.GetUserByToken(token))
            .Returns(user);

        var context = BuildContext(token.ToString());

        _filter.OnAuthorization(context);

        Assert.AreEqual(user, context.HttpContext.Items[HttpContextItemKey.UserLogged]);
    }
}

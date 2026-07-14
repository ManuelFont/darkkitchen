using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DarkKitchen.WebApi.Test.Filters;

[TestClass]
public sealed class ExceptionFilterTests
{
    private ExceptionFilter _filter = null!;
    private Mock<IWebHostEnvironment> _environment = null!;

    [TestInitialize]
    public void Setup()
    {
        _environment = new Mock<IWebHostEnvironment>();
        _environment.Setup(e => e.EnvironmentName).Returns("Production");
        _filter = new ExceptionFilter(_environment.Object);
    }

    private static ExceptionContext BuildContext(Exception exception)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, [])
        {
            Exception = exception
        };
    }

    [TestMethod]
    public void Given_InvalidArgumentException_When_OnException_Then_Returns400()
    {
        var context = BuildContext(new InvalidArgumentException("Bad argument"));

        _filter.OnException(context);

        Assert.AreEqual(400, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_InvalidArgumentException_When_OnException_Then_ResponseContainsInnerCodeAndMessage()
    {
        var context = BuildContext(new InvalidArgumentException("Bad argument"));

        _filter.OnException(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("InvalidArgument", body.InnerCode);
        Assert.AreEqual("Bad argument", body.Message);
    }

    [TestMethod]
    public void Given_ResourceNotFoundException_When_OnException_Then_Returns404()
    {
        var context = BuildContext(new ResourceNotFoundException("User", Guid.NewGuid()));

        _filter.OnException(context);

        Assert.AreEqual(404, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_DuplicateResourceException_When_OnException_Then_Returns409()
    {
        var context = BuildContext(new DuplicateResourceException("User", "Email", "john@x.com"));

        _filter.OnException(context);

        Assert.AreEqual(409, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_UnknownException_When_OnException_Then_Returns500()
    {
        var context = BuildContext(new Exception("Internal server error"));

        _filter.OnException(context);

        Assert.AreEqual(500, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_UnknownException_When_OnException_Then_DoesNotLeakInternalMessage()
    {
        var context = BuildContext(new Exception("Internal server error"));

        _filter.OnException(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("There was an error when processing the request", body.Message);
    }

    [TestMethod]
    public void Given_AnyHandledException_When_OnException_Then_ExceptionHandledIsTrue()
    {
        var context = BuildContext(new InvalidArgumentException("Bad argument"));

        _filter.OnException(context);

        Assert.IsTrue(context.ExceptionHandled);
    }
}

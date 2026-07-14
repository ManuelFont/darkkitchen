using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace DarkKitchen.WebApi.Test.Filters;

[TestClass]
public sealed class ModelStateResponseFactoryTests
{
    [TestMethod]
    public void Given_ModelStateWithError_When_Handle_Then_Returns400()
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        actionContext.ModelState.AddModelError("FirstName", "The FirstName field is required.");

        var result = (BadRequestObjectResult)ModelStateResponseFactory.Handle(actionContext);

        Assert.AreEqual(400, result.StatusCode);
    }

    [TestMethod]
    public void Given_ModelStateWithError_When_Handle_Then_ResponseContainsInnerCodeAndMessage()
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        actionContext.ModelState.AddModelError("FirstName", "The FirstName field is required.");

        var result = (BadRequestObjectResult)ModelStateResponseFactory.Handle(actionContext);

        var body = (ErrorResponse)result.Value!;
        Assert.AreEqual("InvalidArgument", body.InnerCode);
        Assert.AreEqual("The FirstName field is required.", body.Message);
    }

    [TestMethod]
    public void Given_EmptyModelState_When_Handle_Then_ReturnsDefaultMessage()
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        var result = (BadRequestObjectResult)ModelStateResponseFactory.Handle(actionContext);

        var body = (ErrorResponse)result.Value!;
        Assert.AreEqual("InvalidArgument", body.InnerCode);
        Assert.AreEqual("Invalid request", body.Message);
    }
}

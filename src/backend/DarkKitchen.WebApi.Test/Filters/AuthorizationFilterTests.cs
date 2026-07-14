using DarkKitchen.Domain.Entities;
using DarkKitchen.WebApi.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DarkKitchen.WebApi.Test.Filters;

[TestClass]
public sealed class AuthorizationFilterTests
{
    private static AuthorizationFilterContext BuildContext(User? user = null, IActionResult? existingResult = null)
    {
        var httpContext = new DefaultHttpContext();
        if(user is not null)
        {
            httpContext.Items[HttpContextItemKey.UserLogged] = user;
        }

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        var context = new AuthorizationFilterContext(actionContext, []);

        if(existingResult is not null)
        {
            context.Result = existingResult;
        }

        return context;
    }

    private static User BuildUserWithPermission(PermissionNames permission)
    {
        var role = new Role(1, "Administrator");
        role.AddPermission(new Permission(1, permission.ToString()));
        var user = new User();
        user.Role = role;
        return user;
    }

    private static User BuildUserWithoutPermission()
    {
        var role = new Role(1, "Customer");
        var user = new User();
        user.Role = role;
        return user;
    }

    [TestMethod]
    public void Given_ResultAlreadySet_When_OnAuthorization_Then_DoesNotOverrideResult()
    {
        var existingResult = new ObjectResult(null) { StatusCode = 401 };
        var context = BuildContext(existingResult: existingResult);
        var filter = new AuthorizationFilter(PermissionNames.CanCreateProduct);

        filter.OnAuthorization(context);

        Assert.AreEqual(existingResult, context.Result);
    }

    [TestMethod]
    public void Given_NoUserInHttpContextItems_When_OnAuthorization_Then_Returns401()
    {
        var context = BuildContext();
        var filter = new AuthorizationFilter(PermissionNames.CanCreateProduct);

        filter.OnAuthorization(context);

        Assert.AreEqual(401, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_NoUserInHttpContextItems_When_OnAuthorization_Then_ReturnsNotAuthenticated()
    {
        var context = BuildContext();
        var filter = new AuthorizationFilter(PermissionNames.CanCreateProduct);

        filter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("UnAuthorized", body.InnerCode);
        Assert.AreEqual("Not authenticated", body.Message);
    }

    [TestMethod]
    public void Given_UserWithoutPermission_When_OnAuthorization_Then_Returns403()
    {
        var user = BuildUserWithoutPermission();
        var context = BuildContext(user);
        var filter = new AuthorizationFilter(PermissionNames.CanCreateProduct);

        filter.OnAuthorization(context);

        Assert.AreEqual(403, ((ObjectResult)context.Result!).StatusCode);
    }

    [TestMethod]
    public void Given_UserWithoutPermission_When_OnAuthorization_Then_ReturnsForbiddenMessage()
    {
        var user = BuildUserWithoutPermission();
        var context = BuildContext(user);
        var filter = new AuthorizationFilter(PermissionNames.CanCreateProduct);

        filter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Forbidden", body.InnerCode);
        Assert.AreEqual($"Missing permission {PermissionNames.CanCreateProduct}", body.Message);
    }

    [TestMethod]
    public void Given_UserWithPermission_When_OnAuthorization_Then_DoesNotSetResult()
    {
        var user = BuildUserWithPermission(PermissionNames.CanCreateProduct);
        var context = BuildContext(user);
        var filter = new AuthorizationFilter(PermissionNames.CanCreateProduct);

        filter.OnAuthorization(context);

        Assert.IsNull(context.Result);
    }
}

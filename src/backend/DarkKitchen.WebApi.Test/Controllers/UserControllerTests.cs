using DarkKitchen.Application.Services.Users;
using DarkKitchen.Application.Services.Users.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.Users;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Users;
using DarkKitchen.WebApi.Responses.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class UserControllerTests
{
    private Mock<IUserService> _userServiceMock = null!;
    private UserController _userController = null!;

    [TestInitialize]
    public void Setup()
    {
        _userServiceMock = new Mock<IUserService>();
        _userController = new UserController(_userServiceMock.Object);
        _userController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [TestMethod]
    public void Register_WithValidRequest_Returns201WithNewUserId()
    {
        var expectedId = Guid.NewGuid();
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "ValidPassword1!2$3%",
            Phone = "099123456"
        };

        _userServiceMock
            .Setup(s => s.Register(It.IsAny<RegisterUserDto>()))
            .Returns(expectedId);

        var result = (CreatedResult)_userController.Register(request);

        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(expectedId, ((CreateUserResponse)result.Value!).Id);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Register_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        var request = new CreateUserRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "ValidPassword1!2$3%",
            Phone = "099123456"
        };

        _userServiceMock
            .Setup(s => s.Register(It.IsAny<RegisterUserDto>()))
            .Throws(new DuplicateResourceException("User", "Email", "john@example.com"));

        _userController.Register(request);
    }

    [TestMethod]
    public void GetUsers_WithValidRequest_Returns200WithUserList()
    {
        var userId = Guid.NewGuid();
        var request = new GetUsersRequest { Search = "John" };

        _userServiceMock
            .Setup(s => s.GetUsers(It.IsAny<GetUsersDto>()))
            .Returns(
            [
                new UserSummaryDto
                {
                    Id = userId,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john@example.com",
                    Phone = "099123456",
                    RoleId = 1,
                    RoleName = "Customer"
                }

            ]);

        var result = (OkObjectResult)_userController.GetUsers(request);
        var response = ((IReadOnlyList<UserSummaryResponse>)result.Value!)[0];

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(userId, response.Id);
        Assert.AreEqual("John", response.FirstName);
        Assert.AreEqual("Customer", response.RoleName);
        _userServiceMock.Verify(s => s.GetUsers(It.Is<GetUsersDto>(d => d.Search == "John")), Times.Once);
    }

    [TestMethod]
    public void GetUsers_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(UserController).GetMethod(nameof(UserController.GetUsers))!;
        var serviceFilter = method
            .GetCustomAttributes(typeof(ServiceFilterAttribute), false)
            .Cast<ServiceFilterAttribute>()
            .Single();
        var authorizationFilter = method
            .GetCustomAttributes(typeof(AuthorizationFilter), false)
            .Cast<AuthorizationFilter>()
            .Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            []);

        var role = new Role(1, "Customer");
        var user = new User { Role = role };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;

        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanGetUsers", body.Message);
    }

    [TestMethod]
    public void Create_WithValidRequest_Returns201WithUserSummary()
    {
        var userId = Guid.NewGuid();
        var request = new CreateAdminUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "ValidPassword1!2$3%",
            Phone = "099123456",
            RoleId = 2
        };

        _userServiceMock
            .Setup(s => s.CreateByAdmin(It.IsAny<CreateAdminUserDto>()))
            .Returns(new UserSummaryDto
            {
                Id = userId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Phone = "099123456",
                RoleId = 2,
                RoleName = "Dispatcher"
            });

        var result = (CreatedResult)_userController.Create(request);
        var response = (UserSummaryResponse)result.Value!;

        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual($"users/{userId}", result.Location);
        Assert.AreEqual(userId, response.Id);
        Assert.AreEqual("Dispatcher", response.RoleName);
    }

    [TestMethod]
    public void Create_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(UserController).GetMethod(nameof(UserController.Create))!;
        var serviceFilter = method
            .GetCustomAttributes(typeof(ServiceFilterAttribute), false)
            .Cast<ServiceFilterAttribute>()
            .Single();
        var authorizationFilter = method
            .GetCustomAttributes(typeof(AuthorizationFilter), false)
            .Cast<AuthorizationFilter>()
            .Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            []);

        var role = new Role(1, "Customer");
        var user = new User { Role = role };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;

        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanCreateUser", body.Message);
    }

    [TestMethod]
    public void Update_WithValidRequest_Returns200WithUpdatedUser()
    {
        var requester = new User();
        var targetId = Guid.NewGuid();
        _userController.HttpContext.Items[HttpContextItemKey.UserLogged] = requester;

        var request = new UpdateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Phone = "099123456",
            RoleId = 2
        };

        _userServiceMock
            .Setup(s => s.Update(requester.Id, targetId, It.IsAny<UpdateUserDto>()))
            .Returns(new UserSummaryDto
            {
                Id = targetId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                Phone = "099123456",
                RoleId = 2,
                RoleName = "Dispatcher"
            });

        var result = (OkObjectResult)_userController.Update(targetId, request);
        var response = (UserSummaryResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(targetId, response.Id);
        Assert.AreEqual("Jane", response.FirstName);
        Assert.AreEqual("Dispatcher", response.RoleName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Update_WhenServiceThrowsInvalidArgument_PropagatesException()
    {
        var requester = new User();
        _userController.HttpContext.Items[HttpContextItemKey.UserLogged] = requester;

        _userServiceMock
            .Setup(s => s.Update(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<UpdateUserDto>()))
            .Throws(new InvalidArgumentException("A user cannot modify themselves"));

        _userController.Update(requester.Id, new UpdateUserRequest
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Phone = "099123456",
            RoleId = 1
        });
    }

    [TestMethod]
    public void Update_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(UserController).GetMethod(nameof(UserController.Update))!;
        var serviceFilter = method
            .GetCustomAttributes(typeof(ServiceFilterAttribute), false)
            .Cast<ServiceFilterAttribute>()
            .Single();
        var authorizationFilter = method
            .GetCustomAttributes(typeof(AuthorizationFilter), false)
            .Cast<AuthorizationFilter>()
            .Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            []);

        var role = new Role(1, "Customer");
        var user = new User { Role = role };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;

        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanUpdateUser", body.Message);
    }

    [TestMethod]
    public void Delete_WithValidRequest_Returns204()
    {
        var requester = new User();
        var targetId = Guid.NewGuid();
        _userController.HttpContext.Items[HttpContextItemKey.UserLogged] = requester;

        var result = (NoContentResult)_userController.Delete(targetId);

        Assert.AreEqual(204, result.StatusCode);
        _userServiceMock.Verify(s => s.Delete(requester.Id, targetId), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Delete_WhenServiceThrowsInvalidArgument_PropagatesException()
    {
        var requester = new User();
        _userController.HttpContext.Items[HttpContextItemKey.UserLogged] = requester;

        _userServiceMock
            .Setup(s => s.Delete(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .Throws(new InvalidArgumentException("A user cannot delete themselves"));

        _userController.Delete(requester.Id);
    }

    [TestMethod]
    public void Delete_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(UserController).GetMethod(nameof(UserController.Delete))!;
        var serviceFilter = method
            .GetCustomAttributes(typeof(ServiceFilterAttribute), false)
            .Cast<ServiceFilterAttribute>()
            .Single();
        var authorizationFilter = method
            .GetCustomAttributes(typeof(AuthorizationFilter), false)
            .Cast<AuthorizationFilter>()
            .Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(
            new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
            []);

        var role = new Role(1, "Customer");
        var user = new User { Role = role };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;

        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanDeleteUser", body.Message);
    }
}

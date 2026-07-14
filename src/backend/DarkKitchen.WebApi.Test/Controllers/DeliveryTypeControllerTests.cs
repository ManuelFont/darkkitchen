using DarkKitchen.Application.Services.DeliveryTypes;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.DeliveryTypes;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.DeliveryTypes;
using DarkKitchen.WebApi.Responses.DeliveryTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class DeliveryTypeControllerTests
{
    private Mock<IDeliveryTypeService> _serviceMock = null!;
    private DeliveryTypeController _controller = null!;
    private DeliveryTypeReadDto _dto = null!;

    [TestInitialize]
    public void SetUp()
    {
        _serviceMock = new Mock<IDeliveryTypeService>();
        _controller = new DeliveryTypeController(_serviceMock.Object);
        _dto = new DeliveryTypeReadDto
        {
            Id = Guid.NewGuid(),
            Name = "Envio express",
            Cost = 250m
        };
    }

    [TestMethod]
    public void GetAll_Returns200WithDeliveryTypes()
    {
        _serviceMock.Setup(s => s.GetAll()).Returns([_dto]);

        var result = (OkObjectResult)_controller.GetAll().Result!;
        var response = (List<DeliveryTypeResponse>)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(1, response.Count);
        AssertDeliveryTypeResponse(_dto, response[0]);
    }

    [TestMethod]
    public void GetById_WithExistingId_Returns200WithDeliveryType()
    {
        _serviceMock.Setup(s => s.GetById(_dto.Id)).Returns(_dto);

        var result = (OkObjectResult)_controller.GetById(_dto.Id).Result!;
        var response = (DeliveryTypeResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        AssertDeliveryTypeResponse(_dto, response);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_WhenServiceThrowsNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetById(id)).Throws(new ResourceNotFoundException("DeliveryType", id));

        _controller.GetById(id);
    }

    [TestMethod]
    public void Create_WithValidRequest_Returns201WithCreatedDeliveryType()
    {
        var request = new DeliveryTypeRequest { Name = "Envio express", Cost = 250m };
        _serviceMock.Setup(s => s.Create(request.ToDto())).Returns(_dto);

        var result = (CreatedAtActionResult)_controller.Create(request).Result!;
        var response = (DeliveryTypeResponse)result.Value!;

        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual(nameof(DeliveryTypeController.GetById), result.ActionName);
        AssertDeliveryTypeResponse(_dto, response);
    }

    [TestMethod]
    [ExpectedException(typeof(DuplicateResourceException))]
    public void Create_WhenServiceThrowsDuplicateResource_PropagatesException()
    {
        var request = new DeliveryTypeRequest { Name = "Envio express", Cost = 250m };
        _serviceMock
            .Setup(s => s.Create(request.ToDto()))
            .Throws(new DuplicateResourceException("DeliveryType", "name", request.Name));

        _controller.Create(request);
    }

    [TestMethod]
    public void Update_WithValidRequest_Returns204()
    {
        var request = new DeliveryTypeRequest { Name = "Envio en el dia", Cost = 200m };

        var result = (NoContentResult)_controller.Update(_dto.Id, request);

        Assert.AreEqual(204, result.StatusCode);
        _serviceMock.Verify(s => s.Update(_dto.Id, request.ToDto()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void Update_WhenServiceThrowsNotFound_PropagatesException()
    {
        var request = new DeliveryTypeRequest { Name = "Envio en el dia", Cost = 200m };
        _serviceMock
            .Setup(s => s.Update(_dto.Id, request.ToDto()))
            .Throws(new ResourceNotFoundException("DeliveryType", _dto.Id));

        _controller.Update(_dto.Id, request);
    }

    [TestMethod]
    public void GetAll_HasAuthenticationAndAuthorizationFilters()
    {
        AssertMissingPermission(nameof(DeliveryTypeController.GetAll), "CanGetDeliveryType");
    }

    [TestMethod]
    public void Create_HasAuthenticationAndAuthorizationFilters()
    {
        AssertMissingPermission(nameof(DeliveryTypeController.Create), "CanCreateDeliveryType");
    }

    [TestMethod]
    public void Update_HasAuthenticationAndAuthorizationFilters()
    {
        AssertMissingPermission(nameof(DeliveryTypeController.Update), "CanUpdateDeliveryType");
    }

    private static void AssertDeliveryTypeResponse(DeliveryTypeReadDto expected, DeliveryTypeResponse actual)
    {
        Assert.AreEqual(expected.Id, actual.Id);
        Assert.AreEqual(expected.Name, actual.Name);
        Assert.AreEqual(expected.Cost, actual.Cost);
    }

    private static void AssertMissingPermission(string methodName, string expectedPermission)
    {
        var method = typeof(DeliveryTypeController).GetMethod(methodName)!;
        var serviceFilter = typeof(DeliveryTypeController)
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
        Assert.AreEqual($"Missing permission {expectedPermission}", body.Message);
    }
}

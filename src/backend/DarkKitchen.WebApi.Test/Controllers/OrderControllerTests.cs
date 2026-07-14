using DarkKitchen.Application.Services.Orders;
using DarkKitchen.Application.Services.Orders.Dtos;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.WebApi.Controllers.Orders;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Orders;
using DarkKitchen.WebApi.Responses.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using OrderStatus = DarkKitchen.Domain.Enums.OrderStatus;
namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class OrderControllerTests
{
    private Mock<IOrderService> _orderServiceMock = null!;
    private OrderController _orderController = null!;
    private Guid _deliveryTypeId;

    [TestInitialize]
    public void Setup()
    {
        _orderServiceMock = new Mock<IOrderService>();
        _orderController = new OrderController(_orderServiceMock.Object);
        _deliveryTypeId = Guid.NewGuid();
    }

    [TestMethod]
    public void Create_WithValidRequest_Returns201WithOrderSummary()
    {
        var user = new User();
        var expectedOrderId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var request = new CreateOrderRequest
        {
            DeliveryTypeId = _deliveryTypeId,
            Address = new AddressRequest
            {
                Street = "Main",
                DoorNumber = 123,
                Apartment = "A"
            },
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 2
                }

            ]
        };

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _orderController.HttpContext.Items[HttpContextItemKey.UserLogged] = user;

        _orderServiceMock
            .Setup(s => s.Create(It.IsAny<CreateOrderDto>()))
            .Returns(new CreateOrderResultDto
            {
                OrderId = expectedOrderId,
                ClientId = user.Id,
                Subtotal = 200m,
                DeliveryCost = 54m,
                Total = 285.48m
            });

        var result = (CreatedResult)_orderController.Create(request);

        Assert.AreEqual(201, result.StatusCode);
        Assert.AreEqual($"orders/{expectedOrderId}", result.Location);
        var response = (CreateOrderResponse)result.Value!;
        Assert.AreEqual(expectedOrderId, response.OrderId);
        Assert.AreEqual(user.Id, response.ClientId);
        Assert.AreEqual(200m, response.Subtotal);
        Assert.AreEqual(54m, response.DeliveryCost);
        Assert.AreEqual(285.48m, response.Total);
        _orderServiceMock.Verify(s => s.Create(It.Is<CreateOrderDto>(dto =>
            dto.ClientId == user.Id &&
            dto.DeliveryTypeId == _deliveryTypeId &&
            dto.Street == "Main" &&
            dto.DoorNumber == 123 &&
            dto.Apartment == "A" &&
            dto.Items.Count == 1 &&
            dto.Items[0].ProductId == productId &&
            dto.Items[0].Quantity == 2)), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WhenServiceThrowsInvalidArgumentException_PropagatesException()
    {
        var user = new User();
        var request = new CreateOrderRequest
        {
            DeliveryTypeId = _deliveryTypeId,
            Address = new AddressRequest
            {
                Street = "Main",
                DoorNumber = 123,
                Apartment = "A"
            },
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = Guid.NewGuid(),
                    Quantity = 0
                }

            ]
        };

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _orderController.HttpContext.Items[HttpContextItemKey.UserLogged] = user;

        _orderServiceMock
            .Setup(s => s.Create(It.IsAny<CreateOrderDto>()))
            .Throws(new InvalidArgumentException("Quantity must be greater than zero"));

        _orderController.Create(request);
    }

    [TestMethod]
    public void GetOrders_WithValidRequest_Returns200WithOrderList()
    {
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var request = new GetOrdersRequest
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 12, 31)
        };

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        _orderServiceMock
            .Setup(s => s.GetOrders(It.IsAny<GetOrdersDto>()))
            .Returns(
            [
                new OrderListItemDto
                {
                    OrderId = orderId,
                    ClientFirstName = "John",
                    ClientLastName = "Doe",
                    ClientEmail = "john@test.com",
                    CreatedAt = createdAt,
                    Status = OrderStatus.Pending,
                    Items =
                    [
                        new OrderListItemProductDto
                        {
                            ProductName = "Burger",
                            Quantity = 2,
                            Price = 100m
                        }

                    ]
                }

            ]);

        var result = (OkObjectResult)_orderController.GetOrders(request);
        var response = ((IReadOnlyList<OrderListItemResponse>)result.Value!)[0];

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual("John", response.ClientFirstName);
        Assert.AreEqual("Pending", response.Status);
        Assert.AreEqual(1, response.Items.Count);
        Assert.AreEqual("Burger", response.Items[0].ProductName);
        Assert.AreEqual(2, response.Items[0].Quantity);
        Assert.AreEqual(100m, response.Items[0].Price);
    }

    [TestMethod]
    public void GetOrders_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.GetOrders))!;
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
        Assert.AreEqual("Missing permission CanGetOrders", body.Message);
    }

    [TestMethod]
    public void GetClientOrders_WithValidRequest_Returns200WithSummaryList()
    {
        var user = new User();
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        var request = new GetClientOrdersRequest();

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _orderController.HttpContext.Items[HttpContextItemKey.UserLogged] = user;

        _orderServiceMock
            .Setup(s => s.GetClientOrders(user.Id, It.IsAny<GetClientOrdersDto>()))
            .Returns(
            [
                new ClientOrderSummaryDto
                {
                    OrderId = orderId,
                    ClientFirstName = "John",
                    ClientLastName = "Doe",
                    ClientEmail = "john@test.com",
                    CreatedAt = createdAt,
                    Status = OrderStatus.Pending,
                    Total = 100m,
                    ProductCount = 2
                }

            ]);

        var result = (OkObjectResult)_orderController.GetClientOrders(request);
        var response = ((IReadOnlyList<ClientOrderSummaryResponse>)result.Value!)[0];

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual("John", response.ClientFirstName);
        Assert.AreEqual("Doe", response.ClientLastName);
        Assert.AreEqual("john@test.com", response.ClientEmail);
        Assert.AreEqual(createdAt, response.CreatedAt);
        Assert.AreEqual("Pending", response.Status);
        Assert.AreEqual(100m, response.Total);
        Assert.AreEqual(2, response.ProductCount);
    }

    [TestMethod]
    public void GetClientOrders_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.GetClientOrders))!;
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
        Assert.AreEqual("Missing permission CanGetClientOrders", body.Message);
    }

    [TestMethod]
    public void Create_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.Create))!;
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
        Assert.AreEqual("Missing permission CanCreateOrder", body.Message);
    }

    [TestMethod]
    public void GetById_WithExistingOrder_Returns200WithDetail()
    {
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        _orderServiceMock
            .Setup(s => s.GetById(orderId))
            .Returns(new OrderDetailDto
            {
                OrderId = orderId,
                ClientFirstName = "John",
                ClientLastName = "Doe",
                ClientEmail = "john@test.com",
                CreatedAt = createdAt,
                Status = OrderStatus.Pending,
                Subtotal = 200m,
                DeliveryCost = 54m,
                Total = 309.88m,
                DeliveryTypeId = _deliveryTypeId,
                DeliveryTypeName = "Envio express",
                Street = "Main",
                DoorNumber = 123,
                Apartment = "A",
                Items =
                [
                    new OrderDetailItemDto
                    {
                        ProductName = "Burger",
                        Quantity = 2,
                        UnitPrice = 100m,
                        Subtotal = 200m
                    }

                ]
            });

        var result = (OkObjectResult)_orderController.GetById(orderId);
        var response = (OrderDetailResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual("John", response.ClientFirstName);
        Assert.AreEqual("Pending", response.Status);
        Assert.AreEqual(_deliveryTypeId, response.DeliveryTypeId);
        Assert.AreEqual("Envio express", response.DeliveryTypeName);
        Assert.AreEqual("Main", response.Street);
        Assert.AreEqual(123, response.DoorNumber);
        Assert.AreEqual("A", response.Apartment);
        Assert.AreEqual(200m, response.Subtotal);
        Assert.AreEqual(54m, response.DeliveryCost);
        Assert.AreEqual(309.88m, response.Total);
        Assert.AreEqual(1, response.Items.Count);
        Assert.AreEqual("Burger", response.Items[0].ProductName);
        Assert.AreEqual(2, response.Items[0].Quantity);
        Assert.AreEqual(100m, response.Items[0].UnitPrice);
        Assert.AreEqual(200m, response.Items[0].Subtotal);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void GetById_WhenOrderNotFound_PropagatesException()
    {
        var id = Guid.NewGuid();

        _orderController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        _orderServiceMock
            .Setup(s => s.GetById(id))
            .Throws(new ResourceNotFoundException("Order", id));

        _orderController.GetById(id);
    }

    [TestMethod]
    public void GetById_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.GetById))!;
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
        Assert.AreEqual("Missing permission CanGetOrderDetail", body.Message);
    }

    [TestMethod]
    public void MarkAsReady_WithValidRequest_Returns200WithStatusChange()
    {
        var orderId = Guid.NewGuid();
        var dto = new OrderStatusChangeDto { OrderId = orderId, Status = OrderStatus.Ready, StatusChangedAt = DateTime.UtcNow };
        _orderServiceMock.Setup(s => s.MarkOrderAsReady(orderId)).Returns(dto);

        var result = (OkObjectResult)_orderController.MarkAsReady(orderId);
        var response = (OrderStatusChangeResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual(OrderStatus.Ready, response.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(ResourceNotFoundException))]
    public void MarkAsReady_WhenOrderNotFound_PropagatesException()
    {
        var orderId = Guid.NewGuid();
        _orderServiceMock.Setup(s => s.MarkOrderAsReady(orderId)).Throws(new ResourceNotFoundException("Order", orderId));

        _orderController.MarkAsReady(orderId);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkAsReady_WhenInvalidTransition_PropagatesException()
    {
        var orderId = Guid.NewGuid();
        _orderServiceMock.Setup(s => s.MarkOrderAsReady(orderId)).Throws(new InvalidArgumentException("Order can only be marked as ready if it is pending"));

        _orderController.MarkAsReady(orderId);
    }

    [TestMethod]
    public void MarkAsReady_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.MarkAsReady))!;
        var serviceFilter = method.GetCustomAttributes(typeof(ServiceFilterAttribute), false).Cast<ServiceFilterAttribute>().Single();
        var authorizationFilter = method.GetCustomAttributes(typeof(AuthorizationFilter), false).Cast<AuthorizationFilter>().Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), []);
        var user = new User { Role = new Role(1, "Customer") };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;
        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanMarkOrderReady", body.Message);
    }

    [TestMethod]
    public void MarkAsDelayed_WithValidRequest_Returns200WithStatusChange()
    {
        var orderId = Guid.NewGuid();
        var dto = new OrderStatusChangeDto { OrderId = orderId, Status = OrderStatus.Delayed, StatusChangedAt = DateTime.UtcNow };
        _orderServiceMock.Setup(s => s.MarkOrderAsDelayed(orderId)).Returns(dto);

        var result = (OkObjectResult)_orderController.MarkAsDelayed(orderId);
        var response = (OrderStatusChangeResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual(OrderStatus.Delayed, response.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkAsDelayed_WhenInvalidTransition_PropagatesException()
    {
        var orderId = Guid.NewGuid();
        _orderServiceMock.Setup(s => s.MarkOrderAsDelayed(orderId)).Throws(new InvalidArgumentException("Order can only be marked as delayed if it is pending"));

        _orderController.MarkAsDelayed(orderId);
    }

    [TestMethod]
    public void MarkAsDelayed_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.MarkAsDelayed))!;
        var serviceFilter = method.GetCustomAttributes(typeof(ServiceFilterAttribute), false).Cast<ServiceFilterAttribute>().Single();
        var authorizationFilter = method.GetCustomAttributes(typeof(AuthorizationFilter), false).Cast<AuthorizationFilter>().Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), []);
        var user = new User { Role = new Role(1, "Customer") };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;
        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanMarkOrderDelayed", body.Message);
    }

    [TestMethod]
    public void Cancel_WithValidRequest_Returns200WithStatusChange()
    {
        var orderId = Guid.NewGuid();
        var dto = new OrderStatusChangeDto { OrderId = orderId, Status = OrderStatus.Cancelled, StatusChangedAt = DateTime.UtcNow };
        _orderServiceMock.Setup(s => s.CancelOrder(orderId)).Returns(dto);

        var result = (OkObjectResult)_orderController.Cancel(orderId);
        var response = (OrderStatusChangeResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual(OrderStatus.Cancelled, response.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Cancel_WhenInvalidTransition_PropagatesException()
    {
        var orderId = Guid.NewGuid();
        _orderServiceMock.Setup(s => s.CancelOrder(orderId)).Throws(new InvalidArgumentException("Order can only be marked as cancelled if it is pending"));

        _orderController.Cancel(orderId);
    }

    [TestMethod]
    public void Cancel_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.Cancel))!;
        var serviceFilter = method.GetCustomAttributes(typeof(ServiceFilterAttribute), false).Cast<ServiceFilterAttribute>().Single();
        var authorizationFilter = method.GetCustomAttributes(typeof(AuthorizationFilter), false).Cast<AuthorizationFilter>().Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), []);
        var user = new User { Role = new Role(1, "Customer") };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;
        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanCancelOrder", body.Message);
    }

    [TestMethod]
    public void MarkAsOnTheWay_WithValidRequest_Returns200WithStatusChange()
    {
        var orderId = Guid.NewGuid();
        var dto = new OrderStatusChangeDto { OrderId = orderId, Status = OrderStatus.OnTheWay, StatusChangedAt = DateTime.UtcNow };
        _orderServiceMock.Setup(s => s.MarkOrderAsOnTheWay(orderId)).Returns(dto);

        var result = (OkObjectResult)_orderController.MarkAsOnTheWay(orderId);
        var response = (OrderStatusChangeResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual(OrderStatus.OnTheWay, response.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkAsOnTheWay_WhenInvalidTransition_PropagatesException()
    {
        var orderId = Guid.NewGuid();
        _orderServiceMock.Setup(s => s.MarkOrderAsOnTheWay(orderId)).Throws(new InvalidArgumentException("Order can only be marked as on the way if it is ready"));

        _orderController.MarkAsOnTheWay(orderId);
    }

    [TestMethod]
    public void MarkAsOnTheWay_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.MarkAsOnTheWay))!;
        var serviceFilter = method.GetCustomAttributes(typeof(ServiceFilterAttribute), false).Cast<ServiceFilterAttribute>().Single();
        var authorizationFilter = method.GetCustomAttributes(typeof(AuthorizationFilter), false).Cast<AuthorizationFilter>().Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), []);
        var user = new User { Role = new Role(1, "Customer") };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;
        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanMarkOrderOnTheWay", body.Message);
    }

    [TestMethod]
    public void Deliver_WithValidRequest_Returns200WithStatusChange()
    {
        var orderId = Guid.NewGuid();
        var dto = new OrderStatusChangeDto { OrderId = orderId, Status = OrderStatus.Delivered, StatusChangedAt = DateTime.UtcNow };
        _orderServiceMock.Setup(s => s.DeliverOrder(orderId)).Returns(dto);

        var result = (OkObjectResult)_orderController.Deliver(orderId);
        var response = (OrderStatusChangeResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual(OrderStatus.Delivered, response.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Deliver_WhenInvalidTransition_PropagatesException()
    {
        var orderId = Guid.NewGuid();
        _orderServiceMock.Setup(s => s.DeliverOrder(orderId)).Throws(new InvalidArgumentException("Order can only be marked as delivered if it is on the way"));

        _orderController.Deliver(orderId);
    }

    [TestMethod]
    public void Deliver_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.Deliver))!;
        var serviceFilter = method.GetCustomAttributes(typeof(ServiceFilterAttribute), false).Cast<ServiceFilterAttribute>().Single();
        var authorizationFilter = method.GetCustomAttributes(typeof(AuthorizationFilter), false).Cast<AuthorizationFilter>().Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), []);
        var user = new User { Role = new Role(1, "Customer") };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;
        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanDeliverOrder", body.Message);
    }

    [TestMethod]
    public void MarkAsNotDelivered_WithValidRequest_Returns200WithStatusChange()
    {
        var orderId = Guid.NewGuid();
        var dto = new OrderStatusChangeDto { OrderId = orderId, Status = OrderStatus.NotDelivered, StatusChangedAt = DateTime.UtcNow };
        _orderServiceMock.Setup(s => s.MarkOrderAsNotDelivered(orderId)).Returns(dto);

        var result = (OkObjectResult)_orderController.MarkAsNotDelivered(orderId);
        var response = (OrderStatusChangeResponse)result.Value!;

        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(orderId, response.OrderId);
        Assert.AreEqual(OrderStatus.NotDelivered, response.Status);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void MarkAsNotDelivered_WhenInvalidTransition_PropagatesException()
    {
        var orderId = Guid.NewGuid();
        _orderServiceMock.Setup(s => s.MarkOrderAsNotDelivered(orderId)).Throws(new InvalidArgumentException("Order can only be marked as not delivered if it is on the way"));

        _orderController.MarkAsNotDelivered(orderId);
    }

    [TestMethod]
    public void MarkAsNotDelivered_HasAuthenticationAndAuthorizationFilters()
    {
        var method = typeof(OrderController).GetMethod(nameof(OrderController.MarkAsNotDelivered))!;
        var serviceFilter = method.GetCustomAttributes(typeof(ServiceFilterAttribute), false).Cast<ServiceFilterAttribute>().Single();
        var authorizationFilter = method.GetCustomAttributes(typeof(AuthorizationFilter), false).Cast<AuthorizationFilter>().Single();

        Assert.AreEqual(typeof(AuthenticationFilter), serviceFilter.ServiceType);

        var context = new AuthorizationFilterContext(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()), []);
        var user = new User { Role = new Role(1, "Customer") };
        context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;
        authorizationFilter.OnAuthorization(context);

        var body = (ErrorResponse)((ObjectResult)context.Result!).Value!;
        Assert.AreEqual("Missing permission CanMarkOrderNotDelivered", body.Message);
    }
}

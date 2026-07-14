using DarkKitchen.Application.Services.Orders;
using DarkKitchen.Domain.Entities;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Orders;
using DarkKitchen.WebApi.Responses.Orders;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Orders;

[ApiController]
[Route("orders")]
public sealed class OrderController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpGet]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanGetOrders)]
    public IActionResult GetOrders([FromQuery] GetOrdersRequest request)
    {
        var result = _orderService.GetOrders(request.ToDto());
        return Ok(result.Select(OrderListItemResponse.FromDto).ToList());
    }

    [HttpGet("my")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanGetClientOrders)]
    public IActionResult GetClientOrders([FromQuery] GetClientOrdersRequest request)
    {
        var user = (User)HttpContext.Items[HttpContextItemKey.UserLogged]!;
        var result = _orderService.GetClientOrders(user.Id, request.ToDto());
        return Ok(result.Select(ClientOrderSummaryResponse.FromDto).ToList());
    }

    [HttpGet("{id}")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanGetOrderDetail)]
    public IActionResult GetById([FromRoute] Guid id)
    {
        var result = _orderService.GetById(id);
        return Ok(OrderDetailResponse.FromDto(result));
    }

    [HttpPost]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanCreateOrder)]
    public IActionResult Create(CreateOrderRequest request)
    {
        var user = (User)HttpContext.Items[HttpContextItemKey.UserLogged]!;
        var result = _orderService.Create(request.ToDto(user.Id));

        return Created($"orders/{result.OrderId}", new CreateOrderResponse(
            result.OrderId,
            result.ClientId,
            result.Subtotal,
            result.DeliveryCost,
            result.Total));
    }

    [HttpPatch("{id}/delayed")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanMarkOrderDelayed)]
    public IActionResult MarkAsDelayed([FromRoute] Guid id)
    {
        var result = _orderService.MarkOrderAsDelayed(id);
        return Ok(OrderStatusChangeResponse.FromDto(result));
    }

    [HttpPatch("{id}/ready")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanMarkOrderReady)]
    public IActionResult MarkAsReady([FromRoute] Guid id)
    {
        var result = _orderService.MarkOrderAsReady(id);
        return Ok(OrderStatusChangeResponse.FromDto(result));
    }

    [HttpPatch("{id}/cancel")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanCancelOrder)]
    public IActionResult Cancel([FromRoute] Guid id)
    {
        var result = _orderService.CancelOrder(id);
        return Ok(OrderStatusChangeResponse.FromDto(result));
    }

    [HttpPatch("{id}/on-the-way")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanMarkOrderOnTheWay)]
    public IActionResult MarkAsOnTheWay([FromRoute] Guid id)
    {
        var result = _orderService.MarkOrderAsOnTheWay(id);
        return Ok(OrderStatusChangeResponse.FromDto(result));
    }

    [HttpPatch("{id}/deliver")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanDeliverOrder)]
    public IActionResult Deliver([FromRoute] Guid id)
    {
        var result = _orderService.DeliverOrder(id);
        return Ok(OrderStatusChangeResponse.FromDto(result));
    }

    [HttpPatch("{id}/not-delivered")]
    [ServiceFilter(typeof(AuthenticationFilter))]
    [AuthorizationFilter(PermissionNames.CanMarkOrderNotDelivered)]
    public IActionResult MarkAsNotDelivered([FromRoute] Guid id)
    {
        var result = _orderService.MarkOrderAsNotDelivered(id);
        return Ok(OrderStatusChangeResponse.FromDto(result));
    }
}

using DarkKitchen.Application.Services.Orders.Dtos;

namespace DarkKitchen.Application.Services.Orders;

public interface IOrderService
{
    CreateOrderResultDto Create(CreateOrderDto dto);
    OrderDetailDto GetById(Guid id);
    IReadOnlyList<ClientOrderSummaryDto> GetClientOrders(Guid clientId, GetClientOrdersDto dto);
    IReadOnlyList<OrderListItemDto> GetOrders(GetOrdersDto dto);
    OrderStatusChangeDto MarkOrderAsReady(Guid orderId);
    OrderStatusChangeDto MarkOrderAsDelayed(Guid orderId);
    OrderStatusChangeDto CancelOrder(Guid orderId);
    OrderStatusChangeDto MarkOrderAsOnTheWay(Guid orderId);
    OrderStatusChangeDto DeliverOrder(Guid orderId);
    OrderStatusChangeDto MarkOrderAsNotDelivered(Guid orderId);
}

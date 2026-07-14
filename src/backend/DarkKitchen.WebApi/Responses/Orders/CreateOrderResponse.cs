namespace DarkKitchen.WebApi.Responses.Orders;

public readonly struct CreateOrderResponse(Guid orderId, Guid clientId, decimal subtotal, decimal deliveryCost, decimal total)
{
    public Guid OrderId { get; } = orderId;
    public Guid ClientId { get; } = clientId;
    public decimal Subtotal { get; } = subtotal;
    public decimal DeliveryCost { get; } = deliveryCost;
    public decimal Total { get; } = total;
}

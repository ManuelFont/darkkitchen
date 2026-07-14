using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Entities;

public sealed class Order
{
    private readonly List<OrderItem> _items = [];

    public Guid Id { get; private set; }
    public Guid ClientId { get; private set; }
    public User Client { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public Guid DeliveryTypeId { get; private set; }
    public DeliveryType DeliveryType { get; private set; } = null!;
    public Address Address { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public DateTime StatusChangedAt { get; private set; }
    public decimal Subtotal => Items.Sum(item => item.Subtotal);
    public decimal ItemsTotalWithPromotions => Items.Sum(item => item.Total);
    public decimal DeliveryCost { get; private set; }
    public decimal Total => (ItemsTotalWithPromotions + DeliveryCost) * (1 + IvaRate);
    private const decimal IvaRate = 0.22m;

    public Order(Guid clientId, DeliveryType deliveryType, Address address, List<OrderItem> items)
    {
        ValidateOrder(clientId, deliveryType, address, items);

        Id = Guid.NewGuid();
        ClientId = clientId;
        CreatedAt = DateTime.UtcNow;
        DeliveryTypeId = deliveryType.Id;
        DeliveryType = deliveryType;
        DeliveryCost = deliveryType.Cost;
        Address = address;
        _items = [.. items];
        Status = OrderStatus.Pending;
        StatusChangedAt = CreatedAt;
    }

    private static void ValidateOrder(Guid clientId, DeliveryType deliveryType, Address address, List<OrderItem> items)
    {
        if(clientId == Guid.Empty)
        {
            throw new ArgumentException("Client Id cannot be empty");
        }

        if(deliveryType == null)
        {
            throw new ArgumentException("Delivery type cannot be null");
        }

        if(address == null)
        {
            throw new ArgumentException("Address cannot be null");
        }

        if(items == null)
        {
            throw new ArgumentException("Items cannot be null");
        }

        if(items.Count <= 0)
        {
            throw new ArgumentException("Items cannot be empty");
        }
    }

    public void MarkAsReady()
    {
        if(Status != OrderStatus.Pending && Status != OrderStatus.Delayed)
        {
            throw new InvalidArgumentException("Order can only be marked as ready if it is pending or delayed");
        }

        Status = OrderStatus.Ready;
        StatusChangedAt = DateTime.UtcNow;
    }

    public void MarkAsCancelled()
    {
        if(Status != OrderStatus.Pending && Status != OrderStatus.Delayed)
        {
            throw new InvalidArgumentException("Order can only be marked as cancelled if it is pending or delayed");
        }

        Status = OrderStatus.Cancelled;
        StatusChangedAt = DateTime.UtcNow;
    }

    public void MarkAsOnTheWay()
    {
        if(Status != OrderStatus.Ready)
        {
            throw new InvalidArgumentException("Order can only be marked as on the way if it is ready");
        }

        Status = OrderStatus.OnTheWay;
        StatusChangedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if(Status != OrderStatus.OnTheWay)
        {
            throw new InvalidArgumentException("Order can only be marked as delivered if it is on the way");
        }

        Status = OrderStatus.Delivered;
        StatusChangedAt = DateTime.UtcNow;
    }

    public void MarkAsNotDelivered()
    {
        if(Status != OrderStatus.OnTheWay)
        {
            throw new InvalidArgumentException("Order can only be marked as not delivered if it is on the way");
        }

        Status = OrderStatus.NotDelivered;
        StatusChangedAt = DateTime.UtcNow;
    }

    public void MarkAsDelayed()
    {
        if(Status != OrderStatus.Pending)
        {
            if(Status == OrderStatus.Delayed)
            {
                throw new InvalidArgumentException("Order is already delayed");
            }

            throw new InvalidArgumentException("Order can only be marked as delayed if it is pending");
        }

        Status = OrderStatus.Delayed;
        StatusChangedAt = DateTime.UtcNow;
    }

    private Order()
    {
        Address = null!;
        Client = null!;
    }
}

namespace DarkKitchen.Domain.Enums;

public enum OrderStatus
{
    /// <summary>Order has been placed and is waiting to be processed.</summary>
    Pending,

    /// <summary>Order is ready for pickup or delivery.</summary>
    Ready,

    /// <summary>Order was cancelled.</summary>
    Cancelled,

    /// <summary>Order is on the way to the customer.</summary>
    OnTheWay,

    /// <summary>Order has been successfully delivered.</summary>
    Delivered,

    /// <summary>Order could not be delivered.</summary>
    NotDelivered,

    /// <summary>Order has been delayed by the dispatcher.</summary>
    Delayed
}

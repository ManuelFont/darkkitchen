using System.ComponentModel.DataAnnotations;

namespace DarkKitchen.WebApi.Requests.Orders;

public sealed record CreateOrderItemRequest
{
    [Required]
    public Guid ProductId { get; init; }

    [Required]
    public int Quantity { get; init; }
}

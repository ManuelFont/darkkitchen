using DarkKitchen.Application.Services.Orders.Dtos;
using DarkKitchen.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Requests.Orders;

public sealed record GetOrdersRequest
{
    [FromQuery]
    public DateTime? DateFrom { get; init; }

    [FromQuery]
    public DateTime? DateTo { get; init; }

    [FromQuery]
    public string? Street { get; init; }

    [FromQuery]
    public OrderStatus? Status { get; init; }

    public GetOrdersDto ToDto() => new()
    {
        DateFrom = DateFrom,
        DateTo = DateTo,
        Street = Street,
        Status = Status
    };
}

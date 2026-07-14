using DarkKitchen.Application.Services.Orders.Dtos;
using DarkKitchen.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Requests.Orders;

public sealed record GetClientOrdersRequest
{
    [FromQuery]
    public DateTime? DateFrom { get; init; }

    [FromQuery]
    public DateTime? DateTo { get; init; }

    [FromQuery]
    public OrderStatus? Status { get; init; }

    public GetClientOrdersDto ToDto() => new()
    {
        DateFrom = DateFrom,
        DateTo = DateTo,
        Status = Status
    };
}

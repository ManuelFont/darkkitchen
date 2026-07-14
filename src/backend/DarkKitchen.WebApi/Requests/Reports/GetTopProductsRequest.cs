using DarkKitchen.Application.Services.Reports.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Requests.Reports;

public sealed record GetTopProductsRequest
{
    [FromQuery]
    public DateTime? DateFrom { get; init; }

    [FromQuery]
    public DateTime? DateTo { get; init; }

    public GetTopProductsDto ToDto() => new()
    {
        DateFrom = DateFrom,
        DateTo = DateTo
    };
}

using DarkKitchen.Application.Services.Reports;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Reports;
using DarkKitchen.WebApi.Responses.Reports;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Reports;

[ApiController]
[Route("reports")]
[ServiceFilter(typeof(AuthenticationFilter))]
public sealed class ReportsController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;

    [HttpGet("top-products")]
    [AuthorizationFilter(PermissionNames.CanGetTopProducts)]
    public IActionResult GetTopProducts([FromQuery] GetTopProductsRequest request)
    {
        var result = _reportService.GetTopSoldProducts(request.ToDto());
        return Ok(result.Select(TopProductResponse.FromDto).ToList());
    }

    [HttpGet("sales")]
    [AuthorizationFilter(PermissionNames.CanGetSalesReport)]
    public IActionResult GetSalesReport([FromQuery] GetSalesReportRequest request)
    {
        var result = _reportService.GetSalesReport(request.ToDto());
        return Ok(SalesReportResponse.FromDto(result));
    }
}

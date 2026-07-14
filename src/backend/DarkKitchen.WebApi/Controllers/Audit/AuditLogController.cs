using DarkKitchen.Application.Services.Audit;
using DarkKitchen.WebApi.Filters;
using DarkKitchen.WebApi.Requests.Audit;
using DarkKitchen.WebApi.Responses.Audit;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Controllers.Audit;

[ApiController]
[Route("audit-logs")]
[ServiceFilter(typeof(AuthenticationFilter))]
public sealed class AuditLogController(IAuditService auditService) : ControllerBase
{
    private readonly IAuditService _auditService = auditService;

    [HttpGet]
    [AuthorizationFilter(PermissionNames.CanGetAuditLog)]
    public IActionResult Search([FromQuery] SearchAuditLogsRequest request)
    {
        var result = _auditService.Search(request.ToDto());
        return Ok(result.Select(AuditLogResponse.FromDto).ToList());
    }

    [HttpGet("summary")]
    [AuthorizationFilter(PermissionNames.CanGetAuditLog)]
    public IActionResult Summary()
    {
        var result = _auditService.GetSummary();
        return Ok(AuditSummaryResponse.FromDto(result));
    }
}

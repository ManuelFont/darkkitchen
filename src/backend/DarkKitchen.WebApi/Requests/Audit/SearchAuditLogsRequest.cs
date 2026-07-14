using DarkKitchen.Application.Services.Audit;
using DarkKitchen.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Requests.Audit;

public sealed record SearchAuditLogsRequest
{
    [FromQuery]
    public string? EntityName { get; init; }

    [FromQuery]
    public string? UserEmail { get; init; }

    [FromQuery]
    public AuditAction? Action { get; init; }

    public SearchAuditLogsDto ToDto() => new()
    {
        EntityName = EntityName,
        UserEmail = UserEmail,
        Action = Action
    };
}

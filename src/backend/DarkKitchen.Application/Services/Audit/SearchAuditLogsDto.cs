using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Application.Services.Audit;

public sealed record SearchAuditLogsDto
{
    public string? EntityName { get; init; }
    public string? UserEmail { get; init; }
    public AuditAction? Action { get; init; }
}

using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Application.Services.Audit;

public sealed record AuditLogReadDto
{
    public required Guid Id { get; init; }
    public required DateTime Timestamp { get; init; }
    public required AuditAction Action { get; init; }
    public required string EntityName { get; init; }
    public required Guid EntityId { get; init; }
    public required string Description { get; init; }
    public required Guid UserId { get; init; }
    public required string UserEmail { get; init; }
}

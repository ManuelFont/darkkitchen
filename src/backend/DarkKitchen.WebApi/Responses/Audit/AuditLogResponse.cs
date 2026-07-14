using DarkKitchen.Application.Services.Audit;

namespace DarkKitchen.WebApi.Responses.Audit;

public sealed record AuditLogResponse
{
    public Guid Id { get; init; }
    public DateTime Timestamp { get; init; }
    public string Action { get; init; } = string.Empty;
    public string EntityName { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;

    public static AuditLogResponse FromDto(AuditLogReadDto dto) => new()
    {
        Id = dto.Id,
        Timestamp = dto.Timestamp,
        Action = dto.Action.ToString(),
        EntityName = dto.EntityName,
        EntityId = dto.EntityId,
        Description = dto.Description,
        UserId = dto.UserId,
        UserEmail = dto.UserEmail
    };
}

using DarkKitchen.Application.Abstractions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Application.Services.Audit;

public sealed class AuditService(IAuditLogRepository auditLogRepo, ICurrentUserContext currentUser) : IAuditService
{
    private readonly IAuditLogRepository _auditLogRepo = auditLogRepo;
    private readonly ICurrentUserContext _currentUser = currentUser;

    public void Record(AuditAction action, string entityName, Guid entityId, string description)
    {
        var log = new AuditLog(action, entityName, entityId, description, _currentUser.UserId, _currentUser.UserEmail);
        _auditLogRepo.Add(log);
    }

    public IReadOnlyList<AuditLogReadDto> Search(SearchAuditLogsDto dto)
    {
        return _auditLogRepo.Search(dto.EntityName, dto.UserEmail, dto.Action)
            .Select(ToDto)
            .ToList();
    }

    public AuditSummaryDto GetSummary()
    {
        var counts = _auditLogRepo.CountByAction();

        var created = counts.GetValueOrDefault(AuditAction.Created);
        var updated = counts.GetValueOrDefault(AuditAction.Updated);
        var deleted = counts.GetValueOrDefault(AuditAction.Deleted);

        return new AuditSummaryDto
        {
            Created = created,
            Updated = updated,
            Deleted = deleted,
            Total = created + updated + deleted
        };
    }

    private static AuditLogReadDto ToDto(AuditLog log) => new()
    {
        Id = log.Id,
        Timestamp = log.Timestamp,
        Action = log.Action,
        EntityName = log.EntityName,
        EntityId = log.EntityId,
        Description = log.Description,
        UserId = log.UserId,
        UserEmail = log.UserEmail
    };
}

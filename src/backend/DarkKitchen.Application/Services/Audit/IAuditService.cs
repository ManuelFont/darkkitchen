using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Application.Services.Audit;

public interface IAuditService
{
    void Record(AuditAction action, string entityName, Guid entityId, string description);
    IReadOnlyList<AuditLogReadDto> Search(SearchAuditLogsDto dto);
    AuditSummaryDto GetSummary();
}

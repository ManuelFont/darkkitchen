using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Repositories.EntityRepositories;

public interface IAuditLogRepository
{
    void Add(AuditLog log);
    IReadOnlyList<AuditLog> Search(string? entityName, string? userEmail, AuditAction? action);
    IReadOnlyDictionary<AuditAction, int> CountByAction();
}

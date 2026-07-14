using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class AuditLogRepository(SqlDbContext dbContext) : IAuditLogRepository
{
    private readonly SqlDbContext _dbContext = dbContext;

    public void Add(AuditLog log)
    {
        _dbContext.AuditLogs.Add(log);
        _dbContext.SaveChanges();
    }

    public IReadOnlyList<AuditLog> Search(string? entityName, string? userEmail, AuditAction? action)
    {
        var query = _dbContext.AuditLogs.AsQueryable();

        if(!string.IsNullOrWhiteSpace(entityName))
        {
            query = query.Where(a => a.EntityName == entityName);
        }

        if(!string.IsNullOrWhiteSpace(userEmail))
        {
            query = query.Where(a => a.UserEmail.Contains(userEmail));
        }

        if(action.HasValue)
        {
            query = query.Where(a => a.Action == action.Value);
        }

        return query
            .OrderByDescending(a => a.Timestamp)
            .ToList();
    }

    public IReadOnlyDictionary<AuditAction, int> CountByAction()
    {
        return _dbContext.AuditLogs
            .GroupBy(a => a.Action)
            .Select(g => new { Action = g.Key, Count = g.Count() })
            .ToDictionary(x => x.Action, x => x.Count);
    }
}

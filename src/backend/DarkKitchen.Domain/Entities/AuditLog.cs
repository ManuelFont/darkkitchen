using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Entities;

public sealed class AuditLog
{
    public Guid Id { get; private set; }
    public DateTime Timestamp { get; private set; }
    public AuditAction Action { get; private set; }
    public string EntityName { get; private set; }
    public Guid EntityId { get; private set; }
    public string Description { get; private set; }
    public Guid UserId { get; private set; }
    public string UserEmail { get; private set; }

    public AuditLog(AuditAction action, string entityName, Guid entityId, string description, Guid userId, string userEmail)
    {
        if(string.IsNullOrWhiteSpace(entityName))
        {
            throw new ArgumentException("Entity name is required");
        }

        if(entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity id is required");
        }

        if(string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description is required");
        }

        if(userId == Guid.Empty)
        {
            throw new ArgumentException("Responsible user id is required");
        }

        if(string.IsNullOrWhiteSpace(userEmail))
        {
            throw new ArgumentException("Responsible user email is required");
        }

        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
        Action = action;
        EntityName = entityName.Trim();
        EntityId = entityId;
        Description = description.Trim();
        UserId = userId;
        UserEmail = userEmail.Trim();
    }

    private AuditLog()
    {
        EntityName = null!;
        Description = null!;
        UserEmail = null!;
    }
}

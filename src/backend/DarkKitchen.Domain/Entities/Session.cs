using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Entities;

public sealed class Session
{
    public Guid Token { get; private set; } = Guid.NewGuid();
    public Guid UserId { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public Session(Guid userId, DateTime expiresAt)
    {
        if(userId == Guid.Empty)
        {
            throw new InvalidArgumentException("Invalid User ID is required");
        }

        if(expiresAt <= DateTime.Now)
        {
            throw new InvalidArgumentException("Invalid expiration date for session");
        }

        UserId = userId;
        ExpiresAt = expiresAt;
    }
}

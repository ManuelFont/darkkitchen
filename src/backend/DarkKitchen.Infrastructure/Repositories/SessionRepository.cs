using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.Sessions;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class SessionRepository(SqlDbContext dbContext) : ISessionRepository
{
    public void Add(Session entity)
    {
        dbContext.Sessions.Add(entity);
        dbContext.SaveChanges();
    }

    public Session? GetById(Guid id)
    {
        return dbContext.Sessions.Find(id);
    }

    public void Delete(Guid id)
    {
        var session = dbContext.Sessions.Find(id)
            ?? throw new ResourceNotFoundException("Session", id);

        dbContext.Sessions.Remove(session);
        dbContext.SaveChanges();
    }
}

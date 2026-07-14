using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Repositories.Sessions;

public interface ISessionRepository : ICreateRepository<Session>, IDeleteRepository<Session>
{
    Session? GetById(Guid token);
}

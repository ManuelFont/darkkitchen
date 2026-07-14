using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Repositories.EntityRepositories;

public interface IRoleRepository
{
    Role? GetById(int id);
    void Update(Role entity);
}

using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Repositories.EntityRepositories;

public interface IUserRepository : IRepository<User>
{
    IReadOnlyList<User> GetByFilters(string? search, string? role);
}

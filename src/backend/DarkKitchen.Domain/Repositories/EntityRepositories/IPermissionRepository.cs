using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Repositories.EntityRepositories;

public interface IPermissionRepository
{
    void Add(Permission entity);
    Permission? GetById(int id);
    IEnumerable<Permission> GetAll();
    bool Exists(Expression<Func<Permission, bool>> predicate);
}

using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class PermissionRepository(SqlDbContext dbContext) : IPermissionRepository
{
    public void Add(Permission entity)
    {
        dbContext.Permissions.Add(entity);
        dbContext.SaveChanges();
    }

    public Permission? GetById(int id)
    {
        return dbContext.Permissions.Find(id);
    }

    public IEnumerable<Permission> GetAll()
    {
        return dbContext.Permissions.ToList();
    }

    public bool Exists(Expression<Func<Permission, bool>> predicate)
    {
        return dbContext.Permissions.Any(predicate);
    }
}

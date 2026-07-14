using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class RoleRepository(SqlDbContext dbContext) : IRoleRepository
{
    public Role? GetById(int id)
    {
        return dbContext.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefault(r => r.RoleId == id);
    }

    public void Update(Role entity)
    {
        var exists = dbContext.Roles.Any(r => r.RoleId == entity.RoleId);
        if(!exists)
        {
            throw new InvalidOperationException("Role does not exist");
        }

        dbContext.Roles.Update(entity);
        dbContext.SaveChanges();
    }
}

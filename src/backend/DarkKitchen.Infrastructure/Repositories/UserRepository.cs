using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class UserRepository(SqlDbContext dbContext) : IUserRepository
{
    public void Add(User entity)
    {
        dbContext.Users.Add(entity);
        dbContext.SaveChanges();
    }

    public User? GetById(Guid id)
    {
        return dbContext.Users
            .Include(u => u.Role)
            .ThenInclude(r => r.RolePermissions)
            .FirstOrDefault(u => u.Id == id);
    }

    public IReadOnlyList<User> GetByFilters(string? search, string? role)
    {
        var query = dbContext.Users
            .Include(u => u.Role)
            .AsQueryable();

        if(!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => u.FirstName.Contains(search) || u.LastName.Contains(search));
        }

        if(!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role.RoleName == role);
        }

        return query.ToList();
    }

    public IEnumerable<User> GetAll()
    {
        return dbContext.Users.ToList();
    }

    public bool Exists(Expression<Func<User, bool>> predicate)
    {
        return dbContext.Users.Any(predicate);
    }

    public void Update(User entity)
    {
        var exists = dbContext.Users.Any(u => u.Id == entity.Id);
        if(!exists)
        {
            throw new InvalidOperationException("User does not exist");
        }

        dbContext.Users.Update(entity);
        dbContext.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var user = dbContext.Users.Find(id);
        if(user == null)
        {
            throw new InvalidOperationException("User does not exist");
        }

        dbContext.Users.Remove(user);
        dbContext.SaveChanges();
    }
}

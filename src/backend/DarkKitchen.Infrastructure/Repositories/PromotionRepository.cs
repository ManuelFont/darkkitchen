using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Repositories;

public class PromotionRepository(SqlDbContext dbContext) : IPromotionRepository
{
    public void Add(Promotion entity)
    {
        dbContext.Promotions.Add(entity);
        dbContext.SaveChanges();
    }

    public Promotion? GetById(Guid id)
    {
        return dbContext.Promotions
            .FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Promotion> GetAll()
    {
        return dbContext.Promotions.ToList();
    }

    public bool Exists(Expression<Func<Promotion, bool>> predicate)
    {
        return dbContext.Promotions.Any(predicate);
    }

    public void Update(Promotion entity)
    {
        var exists = dbContext.Promotions.Any(p => p.Id == entity.Id);
        if(!exists)
        {
            throw new ResourceNotFoundException("Promotion", entity.Id);
        }

        dbContext.Promotions.Update(entity);
        dbContext.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var promotion = dbContext.Promotions.Find(id);
        if(promotion == null)
        {
            throw new ResourceNotFoundException("Promotion", id);
        }

        dbContext.Promotions.Remove(promotion);
        dbContext.SaveChanges();
    }

    public IReadOnlyList<Promotion> GetByProduct(Guid productId)
    {
        var product = dbContext.Products
            .Include(p => p.Promotions)
            .FirstOrDefault(p => p.Id == productId);
        return product?.Promotions.ToList() ?? [];
    }

    public IReadOnlyList<Promotion> GetByCategory(Guid categoryId)
    {
        return dbContext.Products
            .Include(p => p.Promotions)
            .Where(p => p.Category.CategoryId == categoryId)
            .SelectMany(p => p.Promotions)
            .Distinct()
            .ToList();
    }
}

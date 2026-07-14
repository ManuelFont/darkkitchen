using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Infrastructure.Repositories;

public class CategoryRepository(SqlDbContext dbContext) : ICategoryRepository
{
    public void Add(ProductCategory entity)
    {
        dbContext.ProductCategories.Add(entity);
        dbContext.SaveChanges();
    }

    public ProductCategory? GetById(Guid id)
    {
        return dbContext.ProductCategories.FirstOrDefault(c => c.CategoryId == id);
    }

    public IEnumerable<ProductCategory> GetAll()
    {
        return dbContext.ProductCategories.ToList();
    }

    public bool Exists(Expression<Func<ProductCategory, bool>> predicate)
    {
        return dbContext.ProductCategories.Any(predicate);
    }

    public void Update(ProductCategory entity)
    {
        var exists = dbContext.ProductCategories.Any(c => c.CategoryId == entity.CategoryId);
        if(!exists)
        {
            throw new InvalidOperationException("Category does not exist");
        }

        dbContext.ProductCategories.Update(entity);
        dbContext.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var category = dbContext.ProductCategories.Find(id);
        if(category == null)
        {
            throw new InvalidOperationException("Category does not exist");
        }

        dbContext.ProductCategories.Remove(category);
        dbContext.SaveChanges();
    }
}

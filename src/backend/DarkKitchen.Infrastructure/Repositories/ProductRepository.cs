using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Repositories;

public class ProductRepository(SqlDbContext dbContext) : IProductRepository
{
    public void Add(Product entity)
    {
        dbContext.Products.Add(entity);
        dbContext.SaveChanges();
    }

    public Product? GetById(Guid id)
    {
        return dbContext.Products
            .Include(p => p.Category)
            .Include("_images")
            .Include(p => p.Promotions)
            .FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Product> GetAll()
    {
        return dbContext.Products.Include(p => p.Category)
            .Include("_images")
            .Include(p => p.Promotions)
            .ToList();
    }

    public bool Exists(Expression<Func<Product, bool>> predicate)
    {
        return dbContext.Products.Any(predicate);
    }

    public bool IsInOrder(Guid productId)
    {
        return dbContext.Set<OrderItem>().Any(item => item.ProductId == productId);
    }

    public void Update(Product entity)
    {
        var exists = dbContext.Products.Any(p => p.Id == entity.Id);
        if(!exists)
        {
            throw new InvalidOperationException("Product does not exist");
        }

        dbContext.Products.Update(entity);
        dbContext.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var product = dbContext.Products.Find(id);
        if(product == null)
        {
            throw new InvalidOperationException("Product does not exist");
        }

        dbContext.Products.Remove(product);
        dbContext.SaveChanges();
    }

    public IReadOnlyList<Product> GetByName(string name)
    {
        var term = name.Trim().ToLower();
        return dbContext.Products
            .Include(p => p.Category)
            .Include("_images")
            .Include(p => p.Promotions)
            .Where(p => p.Name.ToLower().Contains(term))
            .ToList();
    }

    public IReadOnlyList<Product> GetByCategory(Guid categoryId)
        => dbContext.Products
            .Include(p => p.Category)
            .Include("_images")
            .Include(p => p.Promotions)
            .Where(p => p.Category.CategoryId == categoryId)
            .ToList();
}

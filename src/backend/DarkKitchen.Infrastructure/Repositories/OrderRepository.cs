using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class OrderRepository(SqlDbContext dbContext) : IOrderRepository
{
    public void Add(Order entity)
    {
        dbContext.Orders.Add(entity);
        dbContext.SaveChanges();
    }

    public Order? GetById(Guid id)
    {
        return dbContext.Orders
            .Include(o => o.Client)
            .Include(o => o.DeliveryType)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefault(o => o.Id == id);
    }

    public IEnumerable<Order> GetAll()
    {
        return dbContext.Orders
            .Include(o => o.Client)
            .Include(o => o.DeliveryType)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .ToList();
    }

    public IReadOnlyList<Order> GetByClient(Guid clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status)
    {
        var query = dbContext.Orders
            .Include(o => o.Client)
            .Include(o => o.DeliveryType)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .Where(o => o.ClientId == clientId);

        if(dateFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= dateFrom.Value);
        }

        if(dateTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= dateTo.Value);
        }

        if(status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return query.ToList();
    }

    public IReadOnlyList<Order> GetByFilters(DateTime? dateFrom, DateTime? dateTo, string? street, OrderStatus? status)
    {
        var query = dbContext.Orders
            .Include(o => o.Client)
            .Include(o => o.DeliveryType)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .AsQueryable();

        if(dateFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= dateFrom.Value);
        }

        if(dateTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= dateTo.Value);
        }

        if(!string.IsNullOrWhiteSpace(street))
        {
            query = query.Where(o => o.Address.Street.Contains(street));
        }

        if(status.HasValue)
        {
            query = query.Where(o => o.Status == status.Value);
        }

        return query.ToList();
    }

    public bool Exists(Expression<Func<Order, bool>> predicate)
    {
        return dbContext.Orders.Any(predicate);
    }

    public void Update(Order entity)
    {
        var exists = dbContext.Orders.Any(o => o.Id == entity.Id);
        if(!exists)
        {
            throw new InvalidOperationException("Order does not exist");
        }

        dbContext.Orders.Update(entity);
        dbContext.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var order = dbContext.Orders.Find(id);
        if(order == null)
        {
            throw new InvalidOperationException("Order does not exist");
        }

        dbContext.Orders.Remove(order);
        dbContext.SaveChanges();
    }
}

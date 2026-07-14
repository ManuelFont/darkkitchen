using System.Linq.Expressions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class DeliveryTypeRepository(SqlDbContext dbContext) : IDeliveryTypeRepository
{
    public void Add(DeliveryType entity)
    {
        dbContext.DeliveryTypes.Add(entity);
        dbContext.SaveChanges();
    }

    public DeliveryType? GetById(Guid id)
    {
        return dbContext.DeliveryTypes.Find(id);
    }

    public IEnumerable<DeliveryType> GetAll()
    {
        return dbContext.DeliveryTypes.ToList();
    }

    public bool Exists(Expression<Func<DeliveryType, bool>> predicate)
    {
        return dbContext.DeliveryTypes.Any(predicate);
    }

    public void Update(DeliveryType entity)
    {
        var exists = dbContext.DeliveryTypes.Any(d => d.Id == entity.Id);
        if(!exists)
        {
            throw new InvalidOperationException("Delivery type does not exist");
        }

        dbContext.DeliveryTypes.Update(entity);
        dbContext.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var deliveryType = dbContext.DeliveryTypes.Find(id);
        if(deliveryType == null)
        {
            throw new InvalidOperationException("Delivery type does not exist");
        }

        dbContext.DeliveryTypes.Remove(deliveryType);
        dbContext.SaveChanges();
    }
}

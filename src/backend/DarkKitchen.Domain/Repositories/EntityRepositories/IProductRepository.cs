using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Repositories.EntityRepositories;

public interface IProductRepository : IRepository<Product>
{
    IReadOnlyList<Product> GetByName(string name);
    IReadOnlyList<Product> GetByCategory(Guid categoryId);
    bool IsInOrder(Guid productId);
}

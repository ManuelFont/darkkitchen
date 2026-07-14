using DarkKitchen.Domain.Entities;

namespace DarkKitchen.Domain.Repositories.EntityRepositories;

public interface IPromotionRepository : IRepository<Promotion>
{
    IReadOnlyList<Promotion> GetByProduct(Guid productId);
    IReadOnlyList<Promotion> GetByCategory(Guid categoryId);
}

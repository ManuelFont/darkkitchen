using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;

namespace DarkKitchen.Domain.Repositories.EntityRepositories;

public interface IOrderRepository : IRepository<Order>
{
    IReadOnlyList<Order> GetByClient(Guid clientId, DateTime? dateFrom, DateTime? dateTo, OrderStatus? status);
    IReadOnlyList<Order> GetByFilters(DateTime? dateFrom, DateTime? dateTo, string? street, OrderStatus? status);
}

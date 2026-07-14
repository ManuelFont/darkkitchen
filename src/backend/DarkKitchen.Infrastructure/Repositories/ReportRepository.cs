using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Reports;
using DarkKitchen.Domain.Repositories.Reports;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Repositories;

public sealed class ReportRepository(SqlDbContext dbContext) : IReportRepository
{
    public IReadOnlyList<TopSoldProduct> GetTopSoldProducts(DateTime? dateFrom, DateTime? dateTo, int top)
    {
        var orders = dbContext.Orders
            .Where(o => o.Status == OrderStatus.Delivered);

        if(dateFrom.HasValue)
        {
            orders = orders.Where(o => o.CreatedAt >= dateFrom.Value);
        }

        if(dateTo.HasValue)
        {
            orders = orders.Where(o => o.CreatedAt <= dateTo.Value);
        }

        return orders
            .SelectMany(o => o.Items)
            .GroupBy(i => new { i.ProductId, i.Product.Name })
            .Select(g => new { g.Key.ProductId, g.Key.Name, Quantity = g.Sum(i => i.Quantity) })
            .OrderByDescending(g => g.Quantity)
            .Take(top)
            .AsEnumerable()
            .Select(g => new TopSoldProduct(g.ProductId, g.Name, g.Quantity))
            .ToList();
    }

    public IReadOnlyList<SaleEntry> GetDeliveredSales(DateTime? dateFrom, DateTime? dateTo)
    {
        var query = dbContext.Orders
            .Include(o => o.Client)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .ThenInclude(p => p.Promotions)
            .Where(o => o.Status == OrderStatus.Delivered);

        if(dateFrom.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= dateFrom.Value);
        }

        if(dateTo.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= dateTo.Value);
        }

        return query
            .AsEnumerable()
            .Select(o => new SaleEntry(
                o.CreatedAt,
                o.ClientId,
                $"{o.Client.FirstName} {o.Client.LastName}",
                o.ItemsTotalWithPromotions,
                o.DeliveryCost,
                o.Total))
            .ToList();
    }
}

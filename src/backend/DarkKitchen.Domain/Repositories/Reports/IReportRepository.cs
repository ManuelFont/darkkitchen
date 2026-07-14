using DarkKitchen.Domain.Reports;

namespace DarkKitchen.Domain.Repositories.Reports;

public interface IReportRepository
{
    IReadOnlyList<TopSoldProduct> GetTopSoldProducts(DateTime? dateFrom, DateTime? dateTo, int top);
    IReadOnlyList<SaleEntry> GetDeliveredSales(DateTime? dateFrom, DateTime? dateTo);
}

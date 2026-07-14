using DarkKitchen.Application.Services.Reports.Dtos;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Reports;
using DarkKitchen.Domain.Repositories.Reports;

namespace DarkKitchen.Application.Services.Reports;

public sealed class ReportService(IReportRepository reportRepo) : IReportService
{
    private const int TopCount = 5;

    private readonly IReportRepository _reportRepo = reportRepo;

    public IReadOnlyList<TopProductDto> GetTopSoldProducts(GetTopProductsDto dto)
    {
        if(dto.DateFrom.HasValue && dto.DateTo.HasValue && dto.DateFrom > dto.DateTo)
        {
            throw new InvalidArgumentException("DateFrom must be before or equal to DateTo");
        }

        return _reportRepo.GetTopSoldProducts(dto.DateFrom, dto.DateTo, TopCount)
            .Select(ToDto)
            .ToList();
    }

    public SalesReportDto GetSalesReport(GetSalesReportDto dto)
    {
        if(dto.DateFrom.HasValue && dto.DateTo.HasValue && dto.DateFrom > dto.DateTo)
        {
            throw new InvalidArgumentException("DateFrom must be before or equal to DateTo");
        }

        var sales = _reportRepo.GetDeliveredSales(dto.DateFrom, dto.DateTo);

        var months = sales
            .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(monthGroup => new MonthlySalesDto
            {
                Year = monthGroup.Key.Year,
                Month = monthGroup.Key.Month,
                Clients = ClientsToDto(monthGroup),
                Subtotal = Breakdown(monthGroup)
            })
            .ToList();

        return new SalesReportDto
        {
            Months = months,
            GrandTotal = Breakdown(sales)
        };
    }

    private static TopProductDto ToDto(TopSoldProduct product) => new()
    {
        ProductId = product.ProductId,
        Name = product.ProductName,
        Quantity = product.QuantitySold
    };

    private static IReadOnlyList<ClientSalesDto> ClientsToDto(IEnumerable<SaleEntry> monthEntries)
    {
        return monthEntries
            .GroupBy(s => new { s.ClientId, s.ClientName })
            .Select(clientGroup => new ClientSalesDto
            {
                ClientId = clientGroup.Key.ClientId,
                ClientName = clientGroup.Key.ClientName,
                Amounts = Breakdown(clientGroup)
            })
            .OrderBy(c => c.ClientName)
            .ToList();
    }

    private static SalesBreakdownDto Breakdown(IEnumerable<SaleEntry> entries)
    {
        var items = entries.Sum(e => e.ItemsTotal);
        var delivery = entries.Sum(e => e.DeliveryCost);
        var total = entries.Sum(e => e.Total);

        return new SalesBreakdownDto
        {
            Items = items,
            Delivery = delivery,
            Iva = total - items - delivery,
            Total = total
        };
    }
}

using DarkKitchen.Application.Services.Reports.Dtos;

namespace DarkKitchen.Application.Services.Reports;

public interface IReportService
{
    IReadOnlyList<TopProductDto> GetTopSoldProducts(GetTopProductsDto dto);
    SalesReportDto GetSalesReport(GetSalesReportDto dto);
}

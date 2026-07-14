using DarkKitchen.Application.Services.Reports.Dtos;
using DarkKitchen.WebApi.Responses.Reports;

namespace DarkKitchen.WebApi.Test.Responses;

[TestClass]
public sealed class SalesReportResponseTests
{
    [TestMethod]
    public void FromDto_MapsAllLevels()
    {
        var clientId = Guid.NewGuid();
        var amounts = Breakdown(3500m, 600m, 902m, 5002m);
        var dto = new SalesReportDto
        {
            Months = [Month(2026, 1, [Client(clientId, "Juan Perez", amounts)], amounts)],
            GrandTotal = amounts
        };

        var response = SalesReportResponse.FromDto(dto);

        Assert.AreEqual(1, response.Months.Count);
        Assert.AreEqual(2026, response.Months[0].Year);
        Assert.AreEqual(1, response.Months[0].Month);
        Assert.AreEqual(1, response.Months[0].Clients.Count);
        Assert.AreEqual(clientId, response.Months[0].Clients[0].ClientId);
        Assert.AreEqual("Juan Perez", response.Months[0].Clients[0].ClientName);
        Assert.AreEqual(3500m, response.Months[0].Clients[0].Amounts.Items);
        Assert.AreEqual(600m, response.Months[0].Clients[0].Amounts.Delivery);
        Assert.AreEqual(902m, response.Months[0].Clients[0].Amounts.Iva);
        Assert.AreEqual(5002m, response.Months[0].Clients[0].Amounts.Total);
        Assert.AreEqual(5002m, response.Months[0].Subtotal.Total);
        Assert.AreEqual(5002m, response.GrandTotal.Total);
    }

    [TestMethod]
    public void FromDto_FormatsPeriodWithZeroPaddedMonth()
    {
        var zero = Breakdown(0m, 0m, 0m, 0m);
        var dto = new SalesReportDto
        {
            Months = [Month(2026, 1, [], zero), Month(2026, 11, [], zero)],
            GrandTotal = zero
        };

        var response = SalesReportResponse.FromDto(dto);

        Assert.AreEqual("2026-01", response.Months[0].Period);
        Assert.AreEqual("2026-11", response.Months[1].Period);
    }

    private static SalesBreakdownDto Breakdown(decimal items, decimal delivery, decimal iva, decimal total) => new()
    {
        Items = items,
        Delivery = delivery,
        Iva = iva,
        Total = total
    };

    private static MonthlySalesDto Month(int year, int month, IReadOnlyList<ClientSalesDto> clients, SalesBreakdownDto subtotal) => new()
    {
        Year = year,
        Month = month,
        Clients = clients,
        Subtotal = subtotal
    };

    private static ClientSalesDto Client(Guid clientId, string clientName, SalesBreakdownDto amounts) => new()
    {
        ClientId = clientId,
        ClientName = clientName,
        Amounts = amounts
    };
}

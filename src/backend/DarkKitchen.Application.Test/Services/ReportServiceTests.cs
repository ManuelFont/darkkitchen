using DarkKitchen.Application.Services.Reports;
using DarkKitchen.Application.Services.Reports.Dtos;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Reports;
using DarkKitchen.Domain.Repositories.Reports;
using Moq;

namespace DarkKitchen.Application.Test.Services;

[TestClass]
public sealed class ReportServiceTests
{
    private const int ExpectedTopCount = 5;

    private Mock<IReportRepository> _reportRepoMock = null!;
    private IReportService _service = null!;
    private GetTopProductsDto _dto = null!;

    [TestInitialize]
    public void Setup()
    {
        _reportRepoMock = new Mock<IReportRepository>();
        _service = new ReportService(_reportRepoMock.Object);
        _dto = new GetTopProductsDto
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 12, 31)
        };
    }

    [TestMethod]
    public void GetTopSoldProducts_RequestsTopFiveForTheGivenRange()
    {
        _reportRepoMock
            .Setup(r => r.GetTopSoldProducts(_dto.DateFrom, _dto.DateTo, ExpectedTopCount))
            .Returns([]);

        _service.GetTopSoldProducts(_dto);

        _reportRepoMock.Verify(
            r => r.GetTopSoldProducts(_dto.DateFrom, _dto.DateTo, ExpectedTopCount),
            Times.Once);
    }

    [TestMethod]
    public void GetTopSoldProducts_MapsRepositoryResultToDtos()
    {
        var firstId = Guid.NewGuid();
        var secondId = Guid.NewGuid();
        _reportRepoMock
            .Setup(r => r.GetTopSoldProducts(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
            .Returns(
            [
                new(firstId, "Classic Burger", 30),
                new(secondId, "Margherita", 20)
            ]);

        var result = _service.GetTopSoldProducts(_dto);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(firstId, result[0].ProductId);
        Assert.AreEqual("Classic Burger", result[0].Name);
        Assert.AreEqual(30, result[0].Quantity);
        Assert.AreEqual(secondId, result[1].ProductId);
        Assert.AreEqual("Margherita", result[1].Name);
        Assert.AreEqual(20, result[1].Quantity);
    }

    [TestMethod]
    public void GetTopSoldProducts_WhenRepositoryReturnsEmpty_ReturnsEmptyList()
    {
        _reportRepoMock
            .Setup(r => r.GetTopSoldProducts(It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<int>()))
            .Returns([]);

        var result = _service.GetTopSoldProducts(_dto);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetTopSoldProducts_WhenDatesAreNull_RequestsTopFiveForAllTime()
    {
        var dto = new GetTopProductsDto { DateFrom = null, DateTo = null };
        _reportRepoMock
            .Setup(r => r.GetTopSoldProducts(null, null, ExpectedTopCount))
            .Returns([]);

        var result = _service.GetTopSoldProducts(dto);

        Assert.AreEqual(0, result.Count);
        _reportRepoMock.Verify(
            r => r.GetTopSoldProducts(null, null, ExpectedTopCount),
            Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void GetTopSoldProducts_WhenDateFromAfterDateTo_ThrowsInvalidArgumentException()
    {
        var invalidDto = new GetTopProductsDto
        {
            DateFrom = new DateTime(2026, 12, 31),
            DateTo = new DateTime(2026, 1, 1)
        };

        _service.GetTopSoldProducts(invalidDto);
    }

    [TestMethod]
    public void GetTopSoldProducts_WhenDatesAreEqual_DoesNotThrow()
    {
        var sameDay = new DateTime(2026, 6, 1);
        var dto = new GetTopProductsDto { DateFrom = sameDay, DateTo = sameDay };
        _reportRepoMock
            .Setup(r => r.GetTopSoldProducts(sameDay, sameDay, ExpectedTopCount))
            .Returns([]);

        var result = _service.GetTopSoldProducts(dto);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetSalesReport_GroupsByMonthAndClientWithSubtotalsAndGrandTotal()
    {
        var juan = Guid.NewGuid();
        var yuri = Guid.NewGuid();
        var sommer = Guid.NewGuid();
        _reportRepoMock
            .Setup(r => r.GetDeliveredSales(null, null))
            .Returns(
            [
                Sale(new DateTime(2026, 1, 10), juan, "Juan Perez", 3000m, 500m),
                Sale(new DateTime(2026, 1, 20), juan, "Juan Perez", 1000m, 100m),
                Sale(new DateTime(2026, 1, 15), yuri, "Yuri Gagarin", 2500m, 300m),
                Sale(new DateTime(2026, 2, 5), juan, "Juan Perez", 800m, 200m),
                Sale(new DateTime(2026, 2, 12), sommer, "Sommer Schutman", 4000m, 600m)
            ]);

        var result = _service.GetSalesReport(new GetSalesReportDto());

        Assert.AreEqual(2, result.Months.Count);
        var january = result.Months[0];
        Assert.AreEqual(2026, january.Year);
        Assert.AreEqual(1, january.Month);
        Assert.AreEqual(2, january.Clients.Count);
        Assert.AreEqual("Juan Perez", january.Clients[0].ClientName);
        Assert.AreEqual(4000m, january.Clients[0].Amounts.Items);
        Assert.AreEqual(600m, january.Clients[0].Amounts.Delivery);
        Assert.AreEqual(5612m, january.Clients[0].Amounts.Total);
        Assert.AreEqual(1012m, january.Clients[0].Amounts.Iva);
        Assert.AreEqual("Yuri Gagarin", january.Clients[1].ClientName);
        Assert.AreEqual(9028m, january.Subtotal.Total);

        var february = result.Months[1];
        Assert.AreEqual(2, february.Month);
        Assert.AreEqual(2, february.Clients.Count);
        Assert.AreEqual(6832m, february.Subtotal.Total);

        Assert.AreEqual(15860m, result.GrandTotal.Total);
        Assert.AreEqual(
            result.GrandTotal.Total,
            result.GrandTotal.Items + result.GrandTotal.Delivery + result.GrandTotal.Iva);
    }

    [TestMethod]
    public void GetSalesReport_OrdersMonthsChronologicallyAcrossYears()
    {
        var client = Guid.NewGuid();
        _reportRepoMock
            .Setup(r => r.GetDeliveredSales(null, null))
            .Returns(
            [
                Sale(new DateTime(2026, 1, 1), client, "Juan Perez", 100m, 10m),
                Sale(new DateTime(2025, 12, 1), client, "Juan Perez", 100m, 10m),
                Sale(new DateTime(2025, 3, 1), client, "Juan Perez", 100m, 10m)
            ]);

        var result = _service.GetSalesReport(new GetSalesReportDto());

        Assert.AreEqual(3, result.Months.Count);
        Assert.AreEqual((2025, 3), (result.Months[0].Year, result.Months[0].Month));
        Assert.AreEqual((2025, 12), (result.Months[1].Year, result.Months[1].Month));
        Assert.AreEqual((2026, 1), (result.Months[2].Year, result.Months[2].Month));
    }

    [TestMethod]
    public void GetSalesReport_OrdersClientsAlphabeticallyWithinMonth()
    {
        var date = new DateTime(2026, 2, 10);
        _reportRepoMock
            .Setup(r => r.GetDeliveredSales(null, null))
            .Returns(
            [
                Sale(date, Guid.NewGuid(), "Sommer Schutman", 5000m, 600m),
                Sale(date, Guid.NewGuid(), "Juan Perez", 800m, 200m)
            ]);

        var result = _service.GetSalesReport(new GetSalesReportDto());

        Assert.AreEqual("Juan Perez", result.Months[0].Clients[0].ClientName);
        Assert.AreEqual("Sommer Schutman", result.Months[0].Clients[1].ClientName);
    }

    [TestMethod]
    public void GetSalesReport_SameClientInTwoMonths_AppearsInBoth()
    {
        var juan = Guid.NewGuid();
        _reportRepoMock
            .Setup(r => r.GetDeliveredSales(null, null))
            .Returns(
            [
                Sale(new DateTime(2026, 1, 10), juan, "Juan Perez", 100m, 10m),
                Sale(new DateTime(2026, 2, 10), juan, "Juan Perez", 200m, 20m)
            ]);

        var result = _service.GetSalesReport(new GetSalesReportDto());

        Assert.AreEqual(2, result.Months.Count);
        Assert.AreEqual(juan, result.Months[0].Clients[0].ClientId);
        Assert.AreEqual(juan, result.Months[1].Clients[0].ClientId);
    }

    [TestMethod]
    public void GetSalesReport_ForwardsOptionalDateRangeToRepository()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 2, 28);
        _reportRepoMock
            .Setup(r => r.GetDeliveredSales(dateFrom, dateTo))
            .Returns([]);

        _service.GetSalesReport(new GetSalesReportDto { DateFrom = dateFrom, DateTo = dateTo });

        _reportRepoMock.Verify(r => r.GetDeliveredSales(dateFrom, dateTo), Times.Once);
    }

    [TestMethod]
    public void GetSalesReport_WhenNoSales_ReturnsEmptyMonthsAndZeroedGrandTotal()
    {
        _reportRepoMock
            .Setup(r => r.GetDeliveredSales(null, null))
            .Returns([]);

        var result = _service.GetSalesReport(new GetSalesReportDto());

        Assert.AreEqual(0, result.Months.Count);
        Assert.AreEqual(0m, result.GrandTotal.Total);
        Assert.AreEqual(0m, result.GrandTotal.Items);
        Assert.AreEqual(0m, result.GrandTotal.Delivery);
        Assert.AreEqual(0m, result.GrandTotal.Iva);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void GetSalesReport_WhenDateFromAfterDateTo_ThrowsInvalidArgumentException()
    {
        _service.GetSalesReport(new GetSalesReportDto
        {
            DateFrom = new DateTime(2026, 12, 31),
            DateTo = new DateTime(2026, 1, 1)
        });
    }

    private static SaleEntry Sale(DateTime createdAt, Guid clientId, string clientName, decimal items, decimal delivery)
    {
        var total = (items + delivery) * 1.22m;
        return new SaleEntry(createdAt, clientId, clientName, items, delivery, total);
    }
}

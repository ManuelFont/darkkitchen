using DarkKitchen.Application.Services.Reports;
using DarkKitchen.Application.Services.Reports.Dtos;
using DarkKitchen.WebApi.Controllers.Reports;
using DarkKitchen.WebApi.Requests.Reports;
using DarkKitchen.WebApi.Responses.Reports;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DarkKitchen.WebApi.Test.Controllers;

[TestClass]
public sealed class ReportsControllerTests
{
    private Mock<IReportService> _reportServiceMock = null!;
    private ReportsController _controller = null!;
    private GetTopProductsRequest _request = null!;
    private TopProductDto _dto = null!;

    [TestInitialize]
    public void Setup()
    {
        _reportServiceMock = new Mock<IReportService>();
        _controller = new ReportsController(_reportServiceMock.Object);
        _request = new GetTopProductsRequest
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 12, 31)
        };
        _dto = new TopProductDto
        {
            ProductId = Guid.NewGuid(),
            Name = "Classic Burger",
            Quantity = 42
        };
    }

    [TestMethod]
    public void GetTopProducts_ReturnsOkWithMappedResponses()
    {
        _reportServiceMock
            .Setup(s => s.GetTopSoldProducts(It.IsAny<GetTopProductsDto>()))
            .Returns([_dto]);

        var result = _controller.GetTopProducts(_request);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        var responses = ok.Value as IReadOnlyList<TopProductResponse>;
        Assert.IsNotNull(responses);
        Assert.AreEqual(1, responses.Count);
        Assert.AreEqual(_dto.ProductId, responses[0].ProductCode);
        Assert.AreEqual("Classic Burger", responses[0].Name);
        Assert.AreEqual(42, responses[0].Quantity);
    }

    [TestMethod]
    public void GetTopProducts_ForwardsRequestDatesToService()
    {
        _reportServiceMock
            .Setup(s => s.GetTopSoldProducts(It.IsAny<GetTopProductsDto>()))
            .Returns([]);

        _controller.GetTopProducts(_request);

        _reportServiceMock.Verify(
            s => s.GetTopSoldProducts(It.Is<GetTopProductsDto>(d =>
                d.DateFrom == _request.DateFrom && d.DateTo == _request.DateTo)),
            Times.Once);
    }

    [TestMethod]
    public void GetTopProducts_WhenServiceReturnsEmpty_ReturnsOkWithEmptyList()
    {
        _reportServiceMock
            .Setup(s => s.GetTopSoldProducts(It.IsAny<GetTopProductsDto>()))
            .Returns([]);

        var result = _controller.GetTopProducts(_request);

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        var responses = ok.Value as IReadOnlyList<TopProductResponse>;
        Assert.IsNotNull(responses);
        Assert.AreEqual(0, responses.Count);
    }

    [TestMethod]
    public void GetSalesReport_ReturnsOkWithMappedResponse()
    {
        var clientId = Guid.NewGuid();
        var amounts = new SalesBreakdownDto { Items = 3500m, Delivery = 600m, Iva = 902m, Total = 5002m };
        var client = new ClientSalesDto { ClientId = clientId, ClientName = "Juan Perez", Amounts = amounts };
        var month = new MonthlySalesDto { Year = 2026, Month = 1, Clients = [client], Subtotal = amounts };
        _reportServiceMock
            .Setup(s => s.GetSalesReport(It.IsAny<GetSalesReportDto>()))
            .Returns(new SalesReportDto { Months = [month], GrandTotal = amounts });

        var result = _controller.GetSalesReport(new GetSalesReportRequest());

        var ok = result as OkObjectResult;
        Assert.IsNotNull(ok);
        var response = ok.Value as SalesReportResponse;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Months.Count);
        Assert.AreEqual("2026-01", response.Months[0].Period);
        Assert.AreEqual("Juan Perez", response.Months[0].Clients[0].ClientName);
        Assert.AreEqual(5002m, response.GrandTotal.Total);
    }

    [TestMethod]
    public void GetSalesReport_ForwardsOptionalDatesToService()
    {
        var dateFrom = new DateTime(2026, 1, 1);
        var dateTo = new DateTime(2026, 2, 28);
        _reportServiceMock
            .Setup(s => s.GetSalesReport(It.IsAny<GetSalesReportDto>()))
            .Returns(EmptyReport());

        _controller.GetSalesReport(new GetSalesReportRequest { DateFrom = dateFrom, DateTo = dateTo });

        _reportServiceMock.Verify(
            s => s.GetSalesReport(It.Is<GetSalesReportDto>(d => d.DateFrom == dateFrom && d.DateTo == dateTo)),
            Times.Once);
    }

    [TestMethod]
    public void GetSalesReport_WithNoDates_ForwardsNullsToService()
    {
        _reportServiceMock
            .Setup(s => s.GetSalesReport(It.IsAny<GetSalesReportDto>()))
            .Returns(EmptyReport());

        _controller.GetSalesReport(new GetSalesReportRequest());

        _reportServiceMock.Verify(
            s => s.GetSalesReport(It.Is<GetSalesReportDto>(d => d.DateFrom == null && d.DateTo == null)),
            Times.Once);
    }

    private static SalesReportDto EmptyReport() => new()
    {
        Months = [],
        GrandTotal = new SalesBreakdownDto { Items = 0m, Delivery = 0m, Iva = 0m, Total = 0m }
    };
}

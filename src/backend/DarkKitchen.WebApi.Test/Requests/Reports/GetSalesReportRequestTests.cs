using DarkKitchen.WebApi.Requests.Reports;

namespace DarkKitchen.WebApi.Test.Requests.Reports;

[TestClass]
public sealed class GetSalesReportRequestTests
{
    [TestMethod]
    public void ToDto_MapsBothDates()
    {
        var request = new GetSalesReportRequest
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 2, 28)
        };

        var dto = request.ToDto();

        Assert.AreEqual(request.DateFrom, dto.DateFrom);
        Assert.AreEqual(request.DateTo, dto.DateTo);
    }

    [TestMethod]
    public void ToDto_WithNoDates_MapsNulls()
    {
        var request = new GetSalesReportRequest();

        var dto = request.ToDto();

        Assert.IsNull(dto.DateFrom);
        Assert.IsNull(dto.DateTo);
    }
}

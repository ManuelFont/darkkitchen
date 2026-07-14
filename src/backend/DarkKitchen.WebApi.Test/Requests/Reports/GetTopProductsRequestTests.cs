using DarkKitchen.WebApi.Requests.Reports;

namespace DarkKitchen.WebApi.Test.Requests.Reports;

[TestClass]
public sealed class GetTopProductsRequestTests
{
    [TestMethod]
    public void ToDto_MapsAllFields()
    {
        var request = new GetTopProductsRequest
        {
            DateFrom = new DateTime(2026, 1, 1),
            DateTo = new DateTime(2026, 12, 31)
        };

        var dto = request.ToDto();

        Assert.AreEqual(request.DateFrom, dto.DateFrom);
        Assert.AreEqual(request.DateTo, dto.DateTo);
    }

    [TestMethod]
    public void ToDto_WhenDatesAreNull_MapsNulls()
    {
        var request = new GetTopProductsRequest();

        var dto = request.ToDto();

        Assert.IsNull(dto.DateFrom);
        Assert.IsNull(dto.DateTo);
    }
}

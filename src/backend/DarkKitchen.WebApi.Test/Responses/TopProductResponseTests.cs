using DarkKitchen.Application.Services.Reports.Dtos;
using DarkKitchen.WebApi.Responses.Reports;

namespace DarkKitchen.WebApi.Test.Responses;

[TestClass]
public sealed class TopProductResponseTests
{
    [TestMethod]
    public void FromDto_MapsAllFields()
    {
        var dto = new TopProductDto
        {
            ProductId = Guid.NewGuid(),
            Name = "Classic Burger",
            Quantity = 42
        };

        var response = TopProductResponse.FromDto(dto);

        Assert.AreEqual(dto.ProductId, response.ProductCode);
        Assert.AreEqual("Classic Burger", response.Name);
        Assert.AreEqual(42, response.Quantity);
    }
}

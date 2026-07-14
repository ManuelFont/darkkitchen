using DarkKitchen.Application.Services.Promotions;
using DarkKitchen.WebApi.Responses.Promotions;

namespace DarkKitchen.WebApi.Test.Responses;

[TestClass]
public class PromotionResponseTests
{
    [TestMethod]
    public void FromDto_MapsAllPropertiesCorrectly()
    {
        var dto = new PromotionReadDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Promotion",
            DiscountPercentage = 15.5m,
            StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31)
        };

        var response = PromotionResponse.FromDto(dto);

        Assert.AreEqual(dto.Id, response.Id);
        Assert.AreEqual(dto.Name, response.Name);
        Assert.AreEqual(dto.DiscountPercentage, response.DiscountPercentage);
        Assert.AreEqual(dto.StartDate, response.StartDate);
        Assert.AreEqual(dto.EndDate, response.EndDate);
    }
}

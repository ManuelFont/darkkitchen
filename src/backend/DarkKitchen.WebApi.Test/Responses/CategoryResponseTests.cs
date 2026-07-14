using DarkKitchen.Application.Services.Categories;
using DarkKitchen.WebApi.Responses.Categories;

namespace DarkKitchen.WebApi.Test.Responses;

[TestClass]
public class CategoryResponseTests
{
    [TestMethod]
    public void FromDto_MapsAllPropertiesCorrectly()
    {
        var dto = new CategoryReadDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Category",
            Description = "Test Description"
        };

        var response = CategoryResponse.FromDto(dto);

        Assert.AreEqual(dto.Id, response.Id);
        Assert.AreEqual(dto.Name, response.Name);
        Assert.AreEqual(dto.Description, response.Description);
    }
}

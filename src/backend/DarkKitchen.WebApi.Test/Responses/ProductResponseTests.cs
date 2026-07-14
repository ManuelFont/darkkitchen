using DarkKitchen.Application.Services.Categories;
using DarkKitchen.Application.Services.Products;
using DarkKitchen.WebApi.Responses.Products;

namespace DarkKitchen.WebApi.Test.Responses;

[TestClass]
public class ProductResponseTests
{
    [TestMethod]
    public void FromDto_MapsAllPropertiesCorrectly()
    {
        var dto = new ProductReadDto
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "Test Description",
            ImagesUrls = ["https://example.com/product.jpg"],
            Price = 9.99m,
            CategoryId = Guid.NewGuid(),
            Category = new CategoryReadDto
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                Description = "Test Category Description"
            },
            ActivePromotion = null,
            Promotions = []
        };

        var response = ProductResponse.FromDto(dto);

        Assert.AreEqual(dto.Id, response.Id);
        Assert.AreEqual(dto.Name, response.Name);
        Assert.AreEqual(dto.Description, response.Description);
        CollectionAssert.AreEqual(dto.ImagesUrls.ToList(), response.ImagesUrls.ToList());
        Assert.AreEqual(dto.Price, response.Price);
        Assert.AreEqual(dto.CategoryId, response.CategoryId);
        Assert.AreEqual(dto.Category.Id, response.Category.Id);
        Assert.AreEqual(dto.Category.Name, response.Category.Name);
        Assert.AreEqual(dto.Category.Description, response.Category.Description);
    }
}

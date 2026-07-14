using System.ComponentModel.DataAnnotations;
using DarkKitchen.WebApi.Requests.Products;

namespace DarkKitchen.WebApi.Test.Requests;

[TestClass]
public sealed class CreateProductRequestTests
{
    [TestMethod]
    public void ToDto_MapsImagesUrls()
    {
        IReadOnlyList<string> imagesUrls =
        [
            "https://example.com/primary.jpg",
            "https://example.com/secondary.jpg"
        ];
        var request = new CreateProductRequest
        {
            Name = "Product",
            Description = "Product description",
            ImagesUrls = imagesUrls,
            Price = 10,
            CategoryId = Guid.NewGuid()
        };

        var dto = request.ToDto();

        CollectionAssert.AreEqual(imagesUrls.ToList(), dto.ImagesUrls.ToList());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(4)]
    public void Validate_WithInvalidImageCount_ShouldFail(int imageCount)
    {
        var request = new CreateProductRequest
        {
            Name = "Product",
            Description = "Product description",
            ImagesUrls = Enumerable.Range(0, imageCount)
                .Select(index => $"https://example.com/image-{index}.jpg")
                .ToList(),
            Price = 10,
            CategoryId = Guid.NewGuid()
        };
        var validationResults = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            validationResults,
            true);

        Assert.IsFalse(isValid);
    }
}

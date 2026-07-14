using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public class ProductTests
{
    private static readonly int OneYearFromToday = DateTime.Today.AddYears(1).Year;
    private static readonly int OneYearBeforeToday = DateTime.Today.AddYears(-1).Year;
    private readonly DateTime _validStart = new(OneYearBeforeToday, 1, 1);
    private readonly DateTime _validEnd = new(OneYearFromToday, 12, 31);
    private ProductCategory _validCategory = null!;

    [TestInitialize]
    public void Initialize()
    {
        _validCategory = new ProductCategory("HotDrinks", "Perfect for winter");
    }

    [TestMethod]
    public void CreateProduct_WithValidData_ShouldSetPropertiesCorrectly()
    {
        var product = new Product("productName", "description", 3, _validCategory, ["https://example.com/image.jpg"]);

        Assert.AreEqual("productName", product.Name);
        Assert.AreEqual("description", product.Description);
        Assert.AreEqual(3, product.Price);
        Assert.AreNotEqual(Guid.Empty, product.Id);
    }

    [TestMethod]
    public void UpdateProduct_WithValidData_ShouldSetPropertiesCorrectly()
    {
        var product = new Product("productName", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.Update("newName", "newDescription", 10, _validCategory, ["https://example.com/image.jpg"]);

        Assert.AreEqual("newName", product.Name);
        Assert.AreEqual("newDescription", product.Description);
        Assert.AreEqual(10, product.Price);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateProduct_WithNullCategory_ShouldThrowException()
    {
        _ = new Product("productName", "description", 1.3m, null!, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Update_WithNullCategory_ShouldThrowException()
    {
        var product = new Product("productName", "description", 1.3m, _validCategory, ["https://example.com/image.jpg"]);
        product.Update("productName", "description", 1.3m, null!, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [DataRow(0.0)]
    [DataRow(-1.0)]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateProduct_WithInvalidPrice_ShouldThrowException(double price)
    {
        _ = new Product("productName", "description", (decimal)price, _validCategory, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [DataRow(0.0)]
    [DataRow(-1.0)]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void UpdateProduct_WithInvalidPrice_ShouldThrowException(double price)
    {
        var product = new Product("productName", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.Update("productName", "description", (decimal)price, _validCategory, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("value1")]
    [DataRow("value.")]
    [DataRow("value$")]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateProduct_WithInvalidName_ShouldThrowException(string name)
    {
        _ = new Product(name, "description", 3, _validCategory, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("value1")]
    [DataRow("value.")]
    [DataRow("value$")]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void UpdateProduct_WithInvalidName_ShouldThrowException(string name)
    {
        var product = new Product("productName", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.Update(name, "description", 3, _validCategory, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("value1")]
    [DataRow("value.")]
    [DataRow("value$")]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateProduct_WithInvalidDescription_ShouldThrowException(string description)
    {
        _ = new Product("productName", description, 3, _validCategory, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("value1")]
    [DataRow("value.")]
    [DataRow("value$")]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void UpdateProduct_WithInvalidDescription_ShouldThrowException(string description)
    {
        var product = new Product("productName", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.Update("productName", description, 3, _validCategory, ["https://example.com/image.jpg"]);
    }

    [TestMethod]
    [DataRow("  value")]
    [DataRow("value  ")]
    [DataRow("  value  ")]
    public void CreateProduct_WithPaddedName_ShouldTrimAndSucceed(string name)
    {
        var product = new Product(name, "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        Assert.AreEqual("value", product.Name);
    }

    [TestMethod]
    [DataRow("  value")]
    [DataRow("value  ")]
    [DataRow("  value  ")]
    public void UpdateProduct_WithPaddedName_ShouldTrimAndSucceed(string name)
    {
        var product = new Product(name, "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        Assert.AreEqual("value", product.Name);
    }

    [TestMethod]
    public void AddPromotion_ShouldAddPromotion()
    {
        var promotion = new Promotion("name", 0.4m, _validStart, _validEnd);
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion);
        Assert.AreEqual(product.Promotions[0], promotion);
    }

    [TestMethod]
    public void RemovePromotion_ShouldRemovePromotion()
    {
        var promotion = new Promotion("name", 0.4m, _validStart, _validEnd);
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion);
        product.RemovePromotion(promotion);
        Assert.AreEqual(0, product.Promotions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void AddPromotion_PromotionNull_ShouldThrow()
    {
        Promotion promotion = null!;
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion);
        Assert.AreEqual(product.Promotions[0], promotion);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void RemovePromotion_PromotionNull_ShouldThrow()
    {
        Promotion promotion = null!;
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.RemovePromotion(promotion);
        Assert.AreEqual(product.Promotions[0], promotion);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void AddPromotion_Twice_ShouldThrow()
    {
        Promotion promotion = null!;
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion);
        product.AddPromotion(promotion);
        Assert.AreEqual(product.Promotions[0], promotion);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void RemovePromotion_NotYetAdded_ShouldThrow()
    {
        var promotion = new Promotion("name", 0.4m, _validStart, _validEnd);
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.RemovePromotion(promotion);
        Assert.AreEqual(product.Promotions[0], promotion);
    }

    [TestMethod]
    public void ActivePromotion_WithActive_ShouldBePromotion()
    {
        var promotion = new Promotion("name", 0.4m, _validStart, _validEnd);
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion);
        Assert.AreEqual(promotion, product.ActivePromotion());
    }

    [TestMethod]
    public void ActivePromotion_WithInactive_ShouldBeNull()
    {
        var promotion = new Promotion("name", 0.4m, _validStart, DateTime.Today.AddDays(-1));
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion);
        Assert.IsNull(product.ActivePromotion());
    }

    [TestMethod]
    public void ActivePromotion_WithNoPromotions_ShouldBeNull()
    {
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        Assert.IsNull(product.ActivePromotion());
    }

    [TestMethod]
    public void ActivePromotion_WithVariousPromotions_ShouldReturnHighestDiscount()
    {
        var promotion1 = new Promotion("name", 0.7m, _validStart, _validEnd);
        var promotion2 = new Promotion("another name", 0.4m, _validStart, _validEnd);
        var product = new Product("name", "description", 3, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion1);
        product.AddPromotion(promotion2);
        Assert.AreEqual(promotion1, product.ActivePromotion());
    }

    [TestMethod]
    public void DiscountedPrice_WithoutPromotion_ShouldReturnCorrectValue()
    {
        var price = 3;
        var product = new Product("name", "description", price, _validCategory, ["https://example.com/image.jpg"]);
        Assert.AreEqual(price, product.DiscountedPrice());
    }

    [TestMethod]
    public void DiscountedPrice_WithPromotion_ShouldReturnCorrectValue()
    {
        var price = 100;
        var discount = 0.7m;
        var expectedPrice = 30;
        var promotion = new Promotion("name", discount, _validStart, _validEnd);
        var product = new Product("name", "description", price, _validCategory, ["https://example.com/image.jpg"]);
        product.AddPromotion(promotion);
        Assert.AreEqual(expectedPrice, product.DiscountedPrice());
    }

    [TestMethod]
    public void CreateProduct_WithThreeImages_ShouldPreserveTheirOrder()
    {
        IReadOnlyList<string> imagesUrls =
        [
            "https://example.com/primary.jpg",
            "https://example.com/secondary.jpg",
            "https://example.com/third.jpg"
        ];

        var product = new Product("name", "description", 3, _validCategory, imagesUrls);

        CollectionAssert.AreEqual(imagesUrls.ToList(), product.ImagesUrls.ToList());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(4)]
    public void CreateProduct_WithInvalidImageCount_ShouldThrowException(int imageCount)
    {
        var imagesUrls = Enumerable.Range(0, imageCount)
            .Select(index => $"https://example.com/image-{index}.jpg")
            .ToList();

        Assert.ThrowsException<InvalidArgumentException>(
            () => new Product("name", "description", 3, _validCategory, imagesUrls));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void CreateProduct_WithBlankImageUrl_ShouldThrowException(string imageUrl)
    {
        Assert.ThrowsException<InvalidArgumentException>(
            () => new Product("name", "description", 3, _validCategory, [imageUrl]));
    }

    [TestMethod]
    public void UpdateProduct_WithImages_ShouldReplaceExistingImages()
    {
        var product = new Product(
            "name",
            "description",
            3,
            _validCategory,
            ["https://example.com/old.jpg"]);
        IReadOnlyList<string> imagesUrls =
        [
            "https://example.com/new-primary.jpg",
            "https://example.com/new-secondary.jpg"
        ];

        product.Update("name", "description", 3, _validCategory, imagesUrls);

        CollectionAssert.AreEqual(imagesUrls.ToList(), product.ImagesUrls.ToList());
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(4)]
    public void UpdateProduct_WithInvalidImageCount_ShouldThrowException(int imageCount)
    {
        var product = new Product(
            "name",
            "description",
            3,
            _validCategory,
            ["https://example.com/image.jpg"]);
        var imagesUrls = Enumerable.Range(0, imageCount)
            .Select(index => $"https://example.com/image-{index}.jpg")
            .ToList();

        Assert.ThrowsException<InvalidArgumentException>(
            () => product.Update("name", "description", 3, _validCategory, imagesUrls));
    }
}

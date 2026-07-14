using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public class ProductCategoryTests
{
    private const int MaxNameLength = 50;
    private const int MaxDescriptionLength = 255;

    [TestMethod]
    public void Constructor_ValidNameAndDescription_CreatesCategory()
    {
        var category = new ProductCategory("Beverages", "Cold and hot drinks");

        Assert.AreEqual("Beverages", category.Name);
        Assert.AreEqual("Cold and hot drinks", category.Description);
        Assert.AreNotEqual(Guid.Empty, category.CategoryId);
    }

    [TestMethod]
    public void Constructor_NameWithLeadingAndTrailingSpaces_TrimsName()
    {
        var category = new ProductCategory("  Name  ", "  Description  ");

        Assert.AreEqual("Name", category.Name);
        Assert.AreEqual("Description", category.Description);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_NullOrEmptyOrWhitespaceName_ThrowsInvalidArgumentException(string name)
    {
        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory(name, "description"));
    }

    [TestMethod]
    public void Constructor_NameExceedsMaxLength_ThrowsInvalidArgumentException()
    {
        var longName = new string('A', MaxNameLength + 1);

        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory(longName, "description"));
    }

    [TestMethod]
    [DataRow("Beverages2")]
    [DataRow("Hot&Cold")]
    [DataRow("Main!")]
    [DataRow("Category_One")]
    public void Constructor_NameWithInvalidCharacters_ThrowsInvalidArgumentException(string name)
    {
        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory(name, "description"));
    }

    [TestMethod]
    public void Constructor_NameWithConsecutiveSpaces_ThrowsInvalidArgumentException()
    {
        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory("Hot  Drinks", "description"));
    }

    [TestMethod]
    public void Constructor_DescriptionWithLeadingAndTrailingSpaces_TrimsDescription()
    {
        var category = new ProductCategory("Beverages", "  Cold drinks.  ");

        Assert.AreEqual("Cold drinks.", category.Description);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_BlankOrWhitespaceDescription_ThrowsInvalidArgumentException(string description)
    {
        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory("Beverages", description));
    }

    [TestMethod]
    public void Constructor_DescriptionExceedsMaxLength_ThrowsInvalidArgumentException()
    {
        var longDescription = new string('A', MaxDescriptionLength + 1);

        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory("name", longDescription));
    }

    [TestMethod]
    [DataRow("Beverages2")]
    [DataRow("Contains\ttab")]
    [DataRow("Contains\nnewline")]
    [DataRow("Contains\rcarriage")]
    public void Constructor_DescriptionWithInvalidCharacters_ThrowsInvalidArgumentException(string description)
    {
        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory("name", description));
    }

    [TestMethod]
    public void Constructor_DescriptionWithConsecutiveSpaces_ThrowsInvalidArgumentException()
    {
        Assert.ThrowsException<InvalidArgumentException>(() => new ProductCategory("name", "Hot  Drinks"));
    }

    [TestMethod]
    public void Update_ValidNameAndDescription_UpdatesBothValues()
    {
        var category = new ProductCategory("Beverages", "Cold drinks");

        category.Update("Desserts", "Sweet treats.");

        Assert.AreEqual("Desserts", category.Name);
        Assert.AreEqual("Sweet treats.", category.Description);
    }

    [TestMethod]
    public void Update_NameWithLeadingAndTrailingSpaces_TrimsName()
    {
        var category = new ProductCategory("name", "description");
        category.Update("  Name  ", "  Description  ");

        Assert.AreEqual("Name", category.Name);
        Assert.AreEqual("Description", category.Description);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Update_NullOrEmptyOrWhitespaceName_ThrowsInvalidArgumentException(string name)
    {
        var category = new ProductCategory("name", "description");
        Assert.ThrowsException<InvalidArgumentException>(() => category.Update(name, "description"));
    }

    [TestMethod]
    public void Update_NameExceedsMaxLength_ThrowsInvalidArgumentException()
    {
        var category = new ProductCategory("name", "description");
        var longName = new string('A', MaxNameLength + 1);

        Assert.ThrowsException<InvalidArgumentException>(() => category.Update(longName, "description"));
    }

    [TestMethod]
    [DataRow("Beverages2")]
    [DataRow("Hot&Cold")]
    [DataRow("Main!")]
    [DataRow("Category_One")]
    public void Update_NameWithInvalidCharacters_ThrowsInvalidArgumentException(string name)
    {
        var category = new ProductCategory("name", "description");
        Assert.ThrowsException<InvalidArgumentException>(() => category.Update(name, "description"));
    }

    [TestMethod]
    public void Update_NameWithConsecutiveSpaces_ThrowsInvalidArgumentException()
    {
        var category = new ProductCategory("name", "description");
        Assert.ThrowsException<InvalidArgumentException>(() => category.Update("Hot  Drinks", "description"));
    }

    [TestMethod]
    public void Update_DescriptionWithLeadingAndTrailingSpaces_TrimsDescription()
    {
        var category = new ProductCategory("Beverages", "Cold drinks");
        category.Update("name", "  Cold drinks.  ");

        Assert.AreEqual("Cold drinks.", category.Description);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Update_BlankOrWhitespaceDescription_ThrowsInvalidArgumentException(string description)
    {
        var category = new ProductCategory("Beverages", "Cold drinks");
        Assert.ThrowsException<InvalidArgumentException>(() => category.Update("Beverages", description));
    }

    [TestMethod]
    public void Update_DescriptionExceedsMaxLength_ThrowsInvalidArgumentException()
    {
        var category = new ProductCategory("Beverages", "Cold drinks");
        var longDescription = new string('A', MaxDescriptionLength + 1);

        Assert.ThrowsException<InvalidArgumentException>(() => category.Update("name", longDescription));
    }

    [TestMethod]
    [DataRow("Beverages2")]
    [DataRow("Contains\ttab")]
    [DataRow("Contains\nnewline")]
    [DataRow("Contains\rcarriage")]
    public void Update_DescriptionWithInvalidCharacters_ThrowsInvalidArgumentException(string description)
    {
        var category = new ProductCategory("Beverages", "Cold drinks");
        Assert.ThrowsException<InvalidArgumentException>(() => category.Update("name", description));
    }

    [TestMethod]
    public void Update_DescriptionWithConsecutiveSpaces_ThrowsInvalidArgumentException()
    {
        var category = new ProductCategory("Beverages", "Cold drinks");
        Assert.ThrowsException<InvalidArgumentException>(() => category.Update("name", "Hot  Drinks"));
    }
}

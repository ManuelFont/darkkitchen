using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Test.ValueObjects;

[TestClass]
public class ImageTests
{
    [TestMethod]
    public void CreateImage_WithValidData_ShouldSetPropertiesCorrectly()
    {
        var image = new Image("https://example.com/image.jpg", 1);

        Assert.AreEqual("https://example.com/image.jpg", image.Url);
        Assert.AreEqual(1, image.Position);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateImage_WithInvalidUrl_ShouldThrowException(string url)
    {
        _ = new Image(url, 0);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(3)]
    [ExpectedException(typeof(ArgumentException))]
    public void CreateImage_WithInvalidPosition_ShouldThrowException(int position)
    {
        _ = new Image("https://example.com/image.jpg", position);
    }
}

using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public class PromotionTests
{
    private readonly DateTime _validStart = new DateTime(2026, 1, 1);
    private readonly DateTime _validEnd = new DateTime(2026, 12, 31);

    [TestMethod]
    public void CreatePromotion_WithValidData_ShouldCreatePromotion()
    {
        var promotion = new Promotion("Black Friday", 0.1m, _validStart, _validEnd);

        Assert.AreEqual("Black Friday", promotion.Name);
        Assert.AreEqual(0.1m, promotion.DiscountPercentage);
        Assert.AreEqual(_validStart, promotion.StartDate);
        Assert.AreEqual(_validEnd, promotion.EndDate);
        Assert.AreNotEqual(Guid.Empty, promotion.Id);
    }

    [TestMethod]
    public void UpdatePromotion_WithValidData_ShouldUpdatePromotion()
    {
        var promotion = new Promotion("Black Friday", 0.1m, _validStart, _validEnd);
        var newStart = new DateTime(2026, 6, 1);
        var newEnd = new DateTime(2026, 6, 30);

        promotion.Update("Cyber Monday", 0.2m, newStart, newEnd);

        Assert.AreEqual("Cyber Monday", promotion.Name);
        Assert.AreEqual(0.2m, promotion.DiscountPercentage);
        Assert.AreEqual(newStart, promotion.StartDate);
        Assert.AreEqual(newEnd, promotion.EndDate);
    }

    [TestMethod]
    [DataRow(null, "Name cannot be null or empty")]
    [DataRow("", "Name cannot be null or empty")]
    [DataRow("   ", "Name cannot be null or empty")]
    [DataRow("value1", "Name cannot contain number")]
    [DataRow("value.", "Name cannot contain punctuation")]
    [DataRow("value$", "Name cannot contain symbol")]
    public void CreatePromotion_WithInvalidName_ShouldThrowException(string name, string exceptionMessage)
    {
        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            _ = new Promotion(name, 0.1m, _validStart, _validEnd));

        Assert.AreEqual(exceptionMessage, ex.Message);
    }

    [TestMethod]
    [DataRow(0, "Discount cannot be zero or negative")]
    [DataRow(-1, "Discount cannot be zero or negative")]
    [DataRow(2, "Discount must be a fraction between 0 and 1, where 1 represents 100%")]
    public void CreatePromotion_WithInvalidDiscountPercentage_ShouldThrowException(int discount, string exceptionMessage)
    {
        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            _ = new Promotion("name", discount, _validStart, _validEnd));

        Assert.AreEqual(exceptionMessage, ex.Message);
    }

    [TestMethod]
    public void CreatePromotion_WithEndDateBeforeStartDate_ShouldThrowException()
    {
        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            _ = new Promotion("name", 0.1m, _validEnd, _validStart));

        Assert.AreEqual("End date cannot be before start date", ex.Message);
    }

    [TestMethod]
    public void IsActive_WhenDateIsWithinRange_ShouldReturnTrue()
    {
        var promotion = new Promotion("name", 0.1m, _validStart, _validEnd);
        Assert.IsTrue(promotion.IsActive(new DateTime(2026, 6, 15)));
    }

    [TestMethod]
    public void IsActive_WhenDateIsBeforeRange_ShouldReturnFalse()
    {
        var promotion = new Promotion("name", 0.1m, _validStart, _validEnd);
        Assert.IsFalse(promotion.IsActive(new DateTime(2025, 12, 31)));
    }

    [TestMethod]
    public void IsActive_WhenDateIsAfterRange_ShouldReturnFalse()
    {
        var promotion = new Promotion("name", 0.1m, _validStart, _validEnd);
        Assert.IsFalse(promotion.IsActive(new DateTime(2027, 1, 1)));
    }

    [TestMethod]
    [DataRow(null, "Name cannot be null or empty")]
    [DataRow("", "Name cannot be null or empty")]
    [DataRow("   ", "Name cannot be null or empty")]
    [DataRow("value1", "Name cannot contain number")]
    [DataRow("value.", "Name cannot contain punctuation")]
    [DataRow("value$", "Name cannot contain symbol")]
    public void UpdatePromotion_WithInvalidName_ShouldThrowException(string name, string exceptionMessage)
    {
        var promotion = new Promotion("Black Friday", 0.1m, _validStart, _validEnd);

        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            promotion.Update(name, 0.1m, _validStart, _validEnd));

        Assert.AreEqual(exceptionMessage, ex.Message);
    }

    [TestMethod]
    [DataRow(0, "Discount cannot be zero or negative")]
    [DataRow(-1, "Discount cannot be zero or negative")]
    [DataRow(2, "Discount must be a fraction between 0 and 1, where 1 represents 100%")]
    public void UpdatePromotion_WithInvalidDiscountPercentage_ShouldThrowException(int discount, string exceptionMessage)
    {
        var promotion = new Promotion("Black Friday", 0.1m, _validStart, _validEnd);

        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            promotion.Update("Black Friday", discount, _validStart, _validEnd));

        Assert.AreEqual(exceptionMessage, ex.Message);
    }

    [TestMethod]
    public void UpdatePromotion_WithEndDateBeforeStartDate_ShouldThrowException()
    {
        var promotion = new Promotion("Black Friday", 0.1m, _validStart, _validEnd);

        var ex = Assert.ThrowsException<InvalidArgumentException>(() =>
            promotion.Update("Black Friday", 0.1m, _validEnd, _validStart));

        Assert.AreEqual("End date cannot be before start date", ex.Message);
    }
}

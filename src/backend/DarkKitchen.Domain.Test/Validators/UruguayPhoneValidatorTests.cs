using DarkKitchen.Domain.Validators;

namespace DarkKitchen.Domain.Test.Validators;

[TestClass]
public sealed class UruguayPhoneValidatorTests
{
    [TestMethod]
    public void IsValid_WithValidPhone_ReturnsTrue()
    {
        var validator = new UruguayPhoneValidator();

        var result = validator.IsValid("099123456");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void IsValid_WithLetters_ReturnsFalse()
    {
        var validator = new UruguayPhoneValidator();

        var result = validator.IsValid("099abc456");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_WithWrongPrefix_ReturnsFalse()
    {
        var validator = new UruguayPhoneValidator();

        var result = validator.IsValid("012345678");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_WithEmptyString_ReturnsFalse()
    {
        var validator = new UruguayPhoneValidator();

        var result = validator.IsValid(string.Empty);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_WithTooShort_ReturnsFalse()
    {
        var validator = new UruguayPhoneValidator();

        var result = validator.IsValid("09912");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void IsValid_WithTooLong_ReturnsFalse()
    {
        var validator = new UruguayPhoneValidator();

        var result = validator.IsValid("0991234567890");

        Assert.IsFalse(result);
    }
}

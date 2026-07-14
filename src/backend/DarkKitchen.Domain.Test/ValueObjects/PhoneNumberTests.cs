using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Validators;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Test.ValueObjects;

[TestClass]
public sealed class PhoneNumberTests
{
    [TestMethod]
    public void Create_ValidUruguayPhoneNumber_SetsValue()
    {
        var phoneValidator = new UruguayPhoneValidator();

        var phoneNumber = PhoneNumber.Create("099123456", phoneValidator);

        Assert.AreEqual("099123456", phoneNumber.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_EmptyUruguayPhoneNumber_ThrowsArgumentException()
    {
        var phoneValidator = new UruguayPhoneValidator();

        PhoneNumber.Create(string.Empty, phoneValidator);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_NullUruguayPhoneNumber_ThrowsArgumentException()
    {
        var phoneValidator = new UruguayPhoneValidator();

        PhoneNumber.Create(null!, phoneValidator);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_ShorterThan9DigitsUruguayPhoneNumber_ThrowsFormatException()
    {
        var phoneValidator = new UruguayPhoneValidator();

        PhoneNumber.Create("09912345", phoneValidator);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_LongerThan9DigitsUruguayPhoneNumber_ThrowsFormatException()
    {
        var phoneValidator = new UruguayPhoneValidator();

        PhoneNumber.Create("09912345678", phoneValidator);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithLettersUruguayPhoneNumber_ThrowsException()
    {
        var phoneValidator = new UruguayPhoneValidator();

        PhoneNumber.Create("099123456a", phoneValidator);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithSymbolsUruguayPhoneNumber_ThrowsException()
    {
        var phoneValidator = new UruguayPhoneValidator();

        PhoneNumber.Create("099123456!", phoneValidator);
    }

    [TestMethod]
    public void Create_WithSpacesUruguayPhoneNumber_SetsValue()
    {
        var phoneValidator = new UruguayPhoneValidator();

        var phoneNumber = PhoneNumber.Create("099 123 456", phoneValidator);

        Assert.AreEqual("099123456", phoneNumber.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithInvalidCountryCodeUruguayPhoneNumber_ThrowsException()
    {
        var phoneValidator = new UruguayPhoneValidator();

        PhoneNumber.Create("199123456", phoneValidator);
    }

    [TestMethod]
    public void FromStorage_WithValidValue_SetsValue()
    {
        var phoneNumber = PhoneNumber.FromStorage("099123456");

        Assert.AreEqual("099123456", phoneNumber.Value);
    }
}

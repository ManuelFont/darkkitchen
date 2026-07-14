using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Test.ValueObjects;

[TestClass]
public sealed class PasswordTests
{
    [TestMethod]
    public void Create_WithValidPassword_SetsValue()
    {
        var password = Password.Create("ValidPassword1!2$3%");

        Assert.AreEqual("ValidPassword1!2$3%", password.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithEmptyPassword_ThrowsException()
    {
        Password.Create(string.Empty);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithTooShort_ThrowsException()
    {
        Password.Create("Short1!");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithTooLong_ThrowsException()
    {
        Password.Create("ThisIsASuperLongPassword1!2$3%4&5*6(7)8");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithNoUppercase_ThrowsException()
    {
        Password.Create("password1!2$3%4&5*6(7)");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithNoLowercase_ThrowsException()
    {
        Password.Create("PASSWORD1!2$3%4&5*6(7)");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithNoDigit_ThrowsException()
    {
        Password.Create("Password@!$%^&*()");
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithNoSymbol_ThrowsException()
    {
        Password.Create("Not1Valid2Password3");
    }

    [TestMethod]
    public void Create_WithPunctuationAsSpecialChar_SetsValue()
    {
        var password = Password.Create("ValidPassword1!@#");

        Assert.AreEqual("ValidPassword1!@#", password.Value);
    }

    [TestMethod]
    public void FromStorage_WithValidValue_SetsValue()
    {
        var password = Password.FromStorage("ValidPassword1!@#");

        Assert.AreEqual("ValidPassword1!@#", password.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void Create_WithSequentialNumbers_ThrowsException()
    {
        Password.Create("Password123456!$%^&*()");
    }
}

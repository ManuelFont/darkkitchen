using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Exceptions;

[TestClass]
public sealed class DuplicateResourceExceptionTests
{
    [TestMethod]
    public void Given_Fields_When_Constructed_Then_MessageContainsAllFields()
    {
        var exception = new DuplicateResourceException("User", "Email", "john@x.com");

        StringAssert.Contains(exception.Message, "User");
        StringAssert.Contains(exception.Message, "Email");
        StringAssert.Contains(exception.Message, "john@x.com");
    }

    [TestMethod]
    public void Given_Fields_When_Constructed_Then_ResourceNamePropertyIsSet()
    {
        var exception = new DuplicateResourceException("User", "Email", "john@x.com");

        Assert.AreEqual("User", exception.ResourceName);
    }

    [TestMethod]
    public void Given_Fields_When_Constructed_Then_ConflictingFieldPropertyIsSet()
    {
        var exception = new DuplicateResourceException("User", "Email", "john@x.com");

        Assert.AreEqual("Email", exception.ConflictingField);
    }

    [TestMethod]
    public void Given_Fields_When_Constructed_Then_ValuePropertyIsSet()
    {
        var exception = new DuplicateResourceException("User", "Email", "john@x.com");

        Assert.AreEqual("john@x.com", exception.Value);
    }
}

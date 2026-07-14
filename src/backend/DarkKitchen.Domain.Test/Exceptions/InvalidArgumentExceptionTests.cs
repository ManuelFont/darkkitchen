using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Exceptions;

[TestClass]
public sealed class InvalidArgumentExceptionTests
{
    [TestMethod]
    public void Given_Message_When_Constructed_Then_MessageIsSet()
    {
        var exception = new InvalidArgumentException("Example message");

        Assert.AreEqual("Example message", exception.Message);
    }

    [TestMethod]
    public void Given_Message_When_Thrown_Then_IsCatchableAsDomainException()
    {
        var exception = new InvalidArgumentException("Example message");

        Assert.IsInstanceOfType<DomainException>(exception);
    }

    [TestMethod]
    public void Given_Message_When_Thrown_Then_IsCatchableAsException()
    {
        var exception = new InvalidArgumentException("Example message");

        Assert.IsInstanceOfType<Exception>(exception);
    }
}

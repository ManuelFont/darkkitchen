using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Exceptions;

[TestClass]
public sealed class ResourceNotFoundExceptionTests
{
    [TestMethod]
    public void Given_ResourceNameAndId_When_Constructed_Then_MessageContainsBoth()
    {
        var id = Guid.NewGuid();
        var exception = new ResourceNotFoundException("User", id);

        StringAssert.Contains(exception.Message, "User");
        StringAssert.Contains(exception.Message, id.ToString());
    }

    [TestMethod]
    public void Given_ResourceNameAndId_When_Constructed_Then_ResourceNamePropertyIsSet()
    {
        var exception = new ResourceNotFoundException("User", Guid.NewGuid());

        Assert.AreEqual("User", exception.ResourceName);
    }

    [TestMethod]
    public void Given_ResourceNameAndId_When_Constructed_Then_IdPropertyIsSet()
    {
        var id = Guid.NewGuid();
        var exception = new ResourceNotFoundException("User", id);

        Assert.AreEqual(id.ToString(), exception.Identifier);
    }
}

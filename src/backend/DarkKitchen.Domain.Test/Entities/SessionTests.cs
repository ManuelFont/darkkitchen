using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public sealed class SessionTests
{
    [TestMethod]
    public void CreateSession_WithValidArgs_SetsTokenToNewGuid()
    {
        var session = new Session(Guid.NewGuid(), DateTime.Now.AddHours(1));

        Assert.AreNotEqual(Guid.Empty, session.Token);
    }

    [TestMethod]
    public void CreateSession_WithValidArgs_SetsUserId()
    {
        var userId = Guid.NewGuid();

        var session = new Session(userId, DateTime.Now.AddHours(1));

        Assert.AreEqual(userId, session.UserId);
    }

    [TestMethod]
    public void CreateSession_WithValidArgs_SetsExpiresAt()
    {
        var expiresAt = DateTime.Now.AddHours(1);

        var session = new Session(Guid.NewGuid(), expiresAt);

        Assert.AreEqual(expiresAt, session.ExpiresAt);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateSession_WithEmptyUserId_ThrowsException()
    {
        new Session(Guid.Empty, DateTime.Now.AddHours(1));
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateSession_WithPastExpiresAt_ThrowsException()
    {
        new Session(Guid.NewGuid(), DateTime.Now.AddHours(-1));
    }
}

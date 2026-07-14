using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.Sessions;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class SessionRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private ISessionRepository _repository = null!;

    [TestInitialize]
    public void Setup()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<SqlDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new SqlDbContext(options);
        _context.Database.EnsureCreated();

        _repository = new SessionRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsSession()
    {
        var session = new Session(Guid.NewGuid(), DateTime.Now.AddHours(1));

        _repository.Add(session);

        var found = _context.Sessions.Find(session.Token);
        Assert.IsNotNull(found);
        Assert.AreEqual(session.UserId, found.UserId);
    }

    [TestMethod]
    public void GetById_ReturnsSession()
    {
        var session = new Session(Guid.NewGuid(), DateTime.Now.AddHours(1));
        _repository.Add(session);

        var found = _repository.GetById(session.Token);

        Assert.IsNotNull(found);
        Assert.AreEqual(session.Token, found.Token);
        Assert.AreEqual(session.UserId, found.UserId);
    }

    [TestMethod]
    public void GetById_ReturnsNullIfSessionDoesNotExist()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Delete_RemovesSession()
    {
        var session = new Session(Guid.NewGuid(), DateTime.Now.AddHours(1));
        _repository.Add(session);

        _repository.Delete(session.Token);

        var found = _context.Sessions.Find(session.Token);
        Assert.IsNull(found);
    }

    [TestMethod]
    [ExpectedException(typeof(DarkKitchen.Domain.Exceptions.ResourceNotFoundException))]
    public void Delete_WithNonExistentId_ThrowsResourceNotFoundException()
    {
        _repository.Delete(Guid.NewGuid());
    }
}

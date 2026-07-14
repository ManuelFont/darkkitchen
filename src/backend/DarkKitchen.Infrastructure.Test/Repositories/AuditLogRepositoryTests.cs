using DarkKitchen.Domain.Constants;
using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Enums;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class AuditLogRepositoryTests
{
    private const string UserEmail = "admin@test.com";

    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IAuditLogRepository _repository = null!;
    private readonly Guid _userId = Guid.NewGuid();

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

        _repository = new AuditLogRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsLog_RetrievableBySearch()
    {
        var entityId = Guid.NewGuid();
        var log = NewLog(AuditAction.Created, AuditedEntityNames.Product, entityId, "Product created: Pizza");

        _repository.Add(log);

        var result = _repository.Search(null, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(entityId, result[0].EntityId);
        Assert.AreEqual(AuditAction.Created, result[0].Action);
        Assert.AreEqual("Product created: Pizza", result[0].Description);
        Assert.AreEqual(_userId, result[0].UserId);
        Assert.AreEqual(UserEmail, result[0].UserEmail);
    }

    [TestMethod]
    public void Search_WithNoFilters_ReturnsAll()
    {
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), "Product created"));
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Promotion, Guid.NewGuid(), "Promotion created"));

        var result = _repository.Search(null, null, null);

        Assert.AreEqual(2, result.Count);
    }

    [TestMethod]
    public void Search_FiltersByEntityName()
    {
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), "Product created"));
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Promotion, Guid.NewGuid(), "Promotion created"));

        var result = _repository.Search(AuditedEntityNames.Product, null, null);

        Assert.AreEqual(1, result.Count);
        Assert.IsTrue(result.All(r => r.EntityName == AuditedEntityNames.Product));
    }

    [TestMethod]
    public void Search_FiltersByAction()
    {
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), "Created"));
        _repository.Add(NewLog(AuditAction.Updated, AuditedEntityNames.Product, Guid.NewGuid(), "Updated"));
        _repository.Add(NewLog(AuditAction.Deleted, AuditedEntityNames.Product, Guid.NewGuid(), "Deleted"));

        var result = _repository.Search(null, null, AuditAction.Updated);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(AuditAction.Updated, result[0].Action);
    }

    [TestMethod]
    public void Search_FiltersByUserEmail_PartialMatch()
    {
        _repository.Add(NewLogWithEmail("alice@darkkitchen.com", "Created by Alice"));
        _repository.Add(NewLogWithEmail("bob@darkkitchen.com", "Created by Bob"));

        var result = _repository.Search(null, "alice", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("alice@darkkitchen.com", result[0].UserEmail);
    }

    [TestMethod]
    public void Search_CombinesFilters()
    {
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), "Product created"));
        _repository.Add(NewLog(AuditAction.Deleted, AuditedEntityNames.Product, Guid.NewGuid(), "Product deleted"));
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Promotion, Guid.NewGuid(), "Promotion created"));

        var result = _repository.Search(AuditedEntityNames.Product, null, AuditAction.Created);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual(AuditedEntityNames.Product, result[0].EntityName);
        Assert.AreEqual(AuditAction.Created, result[0].Action);
    }

    [TestMethod]
    public void Search_OrdersByTimestampDescending()
    {
        var older = NewLog(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), "First");
        _repository.Add(older);
        Thread.Sleep(5);
        var newer = NewLog(AuditAction.Updated, AuditedEntityNames.Product, Guid.NewGuid(), "Second");
        _repository.Add(newer);

        var result = _repository.Search(null, null, null);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(newer.Id, result[0].Id);
        Assert.AreEqual(older.Id, result[1].Id);
    }

    [TestMethod]
    public void CountByAction_ReturnsCountPerAction()
    {
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), "Created 1"));
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Promotion, Guid.NewGuid(), "Created 2"));
        _repository.Add(NewLog(AuditAction.Updated, AuditedEntityNames.Product, Guid.NewGuid(), "Updated"));
        _repository.Add(NewLog(AuditAction.Deleted, AuditedEntityNames.Product, Guid.NewGuid(), "Deleted"));

        var result = _repository.CountByAction();

        Assert.AreEqual(2, result[AuditAction.Created]);
        Assert.AreEqual(1, result[AuditAction.Updated]);
        Assert.AreEqual(1, result[AuditAction.Deleted]);
    }

    [TestMethod]
    public void CountByAction_OmitsActionsWithoutLogs()
    {
        _repository.Add(NewLog(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), "Created"));

        var result = _repository.CountByAction();

        Assert.AreEqual(1, result[AuditAction.Created]);
        Assert.IsFalse(result.ContainsKey(AuditAction.Updated));
        Assert.IsFalse(result.ContainsKey(AuditAction.Deleted));
    }

    [TestMethod]
    public void CountByAction_WithNoLogs_ReturnsEmpty()
    {
        var result = _repository.CountByAction();

        Assert.AreEqual(0, result.Count);
    }

    private AuditLog NewLog(AuditAction action, string entityName, Guid entityId, string description) =>
        new(action, entityName, entityId, description, _userId, UserEmail);

    private AuditLog NewLogWithEmail(string userEmail, string description) =>
        new(AuditAction.Created, AuditedEntityNames.Product, Guid.NewGuid(), description, _userId, userEmail);
}

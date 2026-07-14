using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class PermissionRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IPermissionRepository _repository = null!;

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

        _repository = new PermissionRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsPermission()
    {
        var permission = new Permission(1000, "CanCreateOrder");

        _repository.Add(permission);

        var found = _context.Permissions.Find(permission.PermissionId);
        Assert.IsNotNull(found);
        Assert.AreEqual("CanCreateOrder", found.PermissionName);
    }

    [TestMethod]
    public void GetById_ReturnsPermission()
    {
        var permission = new Permission(1000, "CanCreateOrder");
        _repository.Add(permission);

        var found = _repository.GetById(permission.PermissionId);

        Assert.IsNotNull(found);
        Assert.AreEqual(permission.PermissionId, found.PermissionId);
        Assert.AreEqual("CanCreateOrder", found.PermissionName);
    }

    [TestMethod]
    public void GetById_ReturnsNullIfPermissionDoesNotExist()
    {
        var result = _repository.GetById(999);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_ReturnsAllPermissions()
    {
        _repository.Add(new Permission(1000, "CanCreateOrder"));
        _repository.Add(new Permission(1001, "CanDeleteOrder"));

        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any(p => p.PermissionName == "CanCreateOrder"));
        Assert.IsTrue(result.Any(p => p.PermissionName == "CanDeleteOrder"));
    }

    [TestMethod]
    public void GetAll_ReturnsOnlySeededPermissions_WhenNoneAdded()
    {
        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(_context.Permissions.Count(), result.Count());
    }

    [TestMethod]
    public void Exists_ReturnsTrueIfPermissionExists()
    {
        _repository.Add(new Permission(1000, "CanCreateOrder"));

        var result = _repository.Exists(p => p.PermissionName == "CanCreateOrder");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_ReturnsFalseIfPermissionDoesNotExist()
    {
        var result = _repository.Exists(p => p.PermissionName == "CanDeleteOrder");
        Assert.IsFalse(result);
    }
}

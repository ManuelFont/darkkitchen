using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class RoleRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IRoleRepository _repository = null!;

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

        _repository = new RoleRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void GetById_ReturnsSeededRole()
    {
        var found = _repository.GetById(1);

        Assert.IsNotNull(found);
        Assert.AreEqual(1, found.RoleId);
        Assert.AreEqual("Customer", found.RoleName);
    }

    [TestMethod]
    public void GetById_ReturnsNullIfRoleDoesNotExist()
    {
        var result = _repository.GetById(999);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetById_IncludesPermissions()
    {
        var permission = new Permission(1000, "CanCreateOrder");
        _context.Permissions.Add(permission);
        _context.SaveChanges();

        var role = _repository.GetById(2)!;
        var beforeCount = role.RolePermissions.Count;
        role.AddPermission(permission);
        _repository.Update(role);

        _context.ChangeTracker.Clear();

        var found = _repository.GetById(2);

        Assert.IsNotNull(found);
        Assert.AreEqual(beforeCount + 1, found.RolePermissions.Count);
        Assert.IsTrue(found.RolePermissions.Any(p => p.PermissionName == "CanCreateOrder"));
    }

    [TestMethod]
    public void Update_UpdatesRole()
    {
        var permission = new Permission(1000, "CanCreateOrder");
        _context.Permissions.Add(permission);
        _context.SaveChanges();

        var role = _repository.GetById(2)!;
        var beforeCount = role.RolePermissions.Count;
        role.AddPermission(permission);
        _repository.Update(role);

        _context.ChangeTracker.Clear();

        var found = _repository.GetById(2);
        Assert.IsNotNull(found);
        Assert.AreEqual(beforeCount + 1, found.RolePermissions.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Update_ThrowsExceptionIfRoleDoesNotExist()
    {
        var role = new Role(999, "NonExistent");
        _repository.Update(role);
    }
}

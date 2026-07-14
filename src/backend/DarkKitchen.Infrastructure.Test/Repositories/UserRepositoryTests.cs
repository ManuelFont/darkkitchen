using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Domain.Validators;
using DarkKitchen.Domain.ValueObjects;
using DarkKitchen.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.Infrastructure.Test.Repositories;

[TestClass]
public class UserRepositoryTests
{
    private SqliteConnection _connection = null!;
    private SqlDbContext _context = null!;
    private IUserRepository _repository = null!;

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

        _repository = new UserRepository(_context);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [TestMethod]
    public void Add_PersistsUser()
    {
        var user = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Add(user);

        var found = _context.Users.Find(user.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("Juan", found.FirstName);
        Assert.AreEqual("juan@example.com", found.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(DbUpdateException))]
    public void Add_ThrowsExceptionIfEmailAlreadyExists()
    {
        var user1 = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        var user2 = new User(
            "Maria",
            "Gomez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Add(user1);
        _repository.Add(user2);
    }

    [TestMethod]
    public void GetById_ReturnsUser()
    {
        var user = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Add(user);

        var found = _repository.GetById(user.Id);

        Assert.IsNotNull(found);
        Assert.AreEqual(user.Id, found.Id);
        Assert.AreEqual("Juan", found.FirstName);
        Assert.AreEqual("juan@example.com", found.Email);
    }

    [TestMethod]
    public void GetById_ReturnsNullIfUserDoesNotExist()
    {
        var result = _repository.GetById(Guid.NewGuid());
        Assert.IsNull(result);
    }

    [TestMethod]
    public void GetAll_ReturnsAllUsers()
    {
        var user1 = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        var user2 = new User(
            "Maria",
            "Gomez",
            "maria@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Add(user1);
        _repository.Add(user2);

        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(8, result.Count());
        Assert.IsTrue(result.Any(u => u.FirstName == "Juan"));
        Assert.IsTrue(result.Any(u => u.FirstName == "Maria"));
    }

    [TestMethod]
    public void GetAll_ReturnsEmptyListIfNoUsersExist()
    {
        var result = _repository.GetAll();

        Assert.IsNotNull(result);
        Assert.AreEqual(6, result.Count());
    }

    [TestMethod]
    public void Exists_ReturnsTrueIfUserExists()
    {
        var user = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Add(user);

        var result = _repository.Exists(u => u.Email == "juan@example.com");
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Exists_ReturnsFalseIfUserDoesNotExist()
    {
        var result = _repository.Exists(u => u.Email == "juan@example.com");
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Update_UpdatesUser()
    {
        var user = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Add(user);

        user.FirstName = "Pedro";
        _repository.Update(user);

        var found = _context.Users.Find(user.Id);
        Assert.IsNotNull(found);
        Assert.AreEqual("Pedro", found.FirstName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Update_ThrowsExceptionIfUserDoesNotExist()
    {
        var user = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Update(user);
    }

    [TestMethod]
    public void Delete_DeletesUser()
    {
        var user = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Add(user);
        _repository.Delete(user.Id);

        var found = _context.Users.Find(user.Id);
        Assert.IsNull(found);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void Delete_ThrowsExceptionIfUserDoesNotExist()
    {
        var user = new User(
            "Juan",
            "Perez",
            "juan@example.com",
            Password.Create("Abcdefghijk#1aaa"),
            PhoneNumber.Create("098123456", new UruguayPhoneValidator()));

        _repository.Delete(user.Id);
    }

    [TestMethod]
    public void GetByFilters_SearchMatchesFirstName()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@example.com"));
        _repository.Add(CreateUser("Maria", "Gomez", "maria@example.com"));

        var result = _repository.GetByFilters("Juan", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Juan", result[0].FirstName);
    }

    [TestMethod]
    public void GetByFilters_SearchMatchesLastName()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@example.com"));
        _repository.Add(CreateUser("Maria", "Gomez", "maria@example.com"));

        var result = _repository.GetByFilters("Perez", null);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("Perez", result[0].LastName);
    }

    [TestMethod]
    public void GetByFilters_WithNoFilters_ReturnsAllUsers()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@example.com"));
        _repository.Add(CreateUser("Maria", "Gomez", "maria@example.com"));

        var result = _repository.GetByFilters(null, null);

        Assert.AreEqual(8, result.Count);
    }

    [TestMethod]
    public void GetByFilters_WithNoMatch_ReturnsEmptyList()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@example.com"));

        var result = _repository.GetByFilters("Carlos", null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetByFilters_FiltersByRole()
    {
        var result = _repository.GetByFilters(null, "Administrator");

        Assert.IsTrue(result.Count > 0);
        Assert.IsTrue(result.All(u => u.Role.RoleName == "Administrator"));
    }

    [TestMethod]
    public void GetByFilters_IncludesRole()
    {
        _repository.Add(CreateUser("Juan", "Perez", "juan@example.com"));

        var result = _repository.GetByFilters("Juan", null);

        Assert.IsNotNull(result[0].Role);
    }

    private static User CreateUser(string firstName, string lastName, string email) =>
    new(firstName, lastName, email,
        Password.Create("Abcdefghijk#1aaa"),
        PhoneNumber.Create("098123456", new UruguayPhoneValidator()));
}

using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Validators;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Test.Entities;

[TestClass]
public sealed class UserTests
{
    [TestMethod]
    public void CreateUser_WithValidName_SetsName()
    {
        var name = "John";
        var user = new User();

        user.FirstName = name;

        Assert.AreEqual(name, user.FirstName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithEmptyFirstName_ThrowsArgumentException()
    {
        var user = new User();

        user.FirstName = string.Empty;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithNumbersInFirstName_ThrowsArgumentException()
    {
        var user = new User();

        user.FirstName = "John123";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithSymbolsInFirstName_ThrowsException()
    {
        var user = new User();

        user.FirstName = "John$";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithPunctuationInFirstName_ThrowsException()
    {
        var user = new User();

        user.FirstName = "John!";
    }

    [TestMethod]
    public void CreateUser_WithValidLastName_SetsLastName()
    {
        var lastName = "Doe";
        var user = new User();

        user.LastName = lastName;

        Assert.AreEqual(lastName, user.LastName);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithEmptyLastName_ThrowsArgumentException()
    {
        var user = new User();

        user.LastName = string.Empty;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithNumbersInLastName_ThrowsArgumentException()
    {
        var user = new User();

        user.LastName = "Doe123";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithSymbolsInLastName_ThrowsException()
    {
        var user = new User();

        user.LastName = "Doe$";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithPunctuationInLastName_ThrowsException()
    {
        var user = new User();

        user.LastName = "Doe!";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithLastNameTooShort_ThrowsException()
    {
        var user = new User();

        user.LastName = "AB";
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithLastNameTooLong_ThrowsException()
    {
        var user = new User();

        user.LastName = new string('A', 26);
    }

    [TestMethod]
    public void CreateUser_WithValidEmail_SetsEmail()
    {
        var email = "john.doe@example.com";
        var user = new User();

        user.Email = email;

        Assert.AreEqual(email, user.Email);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithEmptyEmail_ThrowsArgumentException()
    {
        var user = new User();

        user.Email = string.Empty;
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithInvalidEmailFormat_ThrowsArgumentException()
    {
        var user = new User();

        user.Email = "john.doe.example.com";
    }

    [TestMethod]
    public void CreateUser_WithValidPhone_SetsPhone()
    {
        var phone = PhoneNumber.Create("099123456", new UruguayPhoneValidator());
        var user = new User();

        user.Phone = phone;

        Assert.AreEqual("099123456", user.Phone.Value);
    }

    [TestMethod]
    public void CreateUser_WithValidPhoneWithSpaces_SetsPhone()
    {
        var phone = PhoneNumber.Create("099 123 456", new UruguayPhoneValidator());
        var user = new User();

        user.Phone = phone;

        Assert.AreEqual("099123456", user.Phone.Value);
    }

    [TestMethod]
    public void CreateUser_WithValidPassword_SetsPassword()
    {
        var password = Password.Create("ValidPassword1!2$3%");
        var user = new User();

        user.Password = password;

        Assert.AreEqual("ValidPassword1!2$3%", user.Password.Value);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidArgumentException))]
    public void CreateUser_WithNullPassword_ThrowsException()
    {
        var user = new User();

        user.Password = null!;
    }

    [TestMethod]
    public void CreateUser_WhenCreated_HasNewGuidId()
    {
        var user = new User();

        Assert.AreNotEqual(Guid.Empty, user.Id);
    }

    [TestMethod]
    public void CreateUser_UsingConstructor_SetsAllProperties()
    {
        var password = Password.Create("ValidPassword1!2$3%");
        var phone = PhoneNumber.Create("099123456", new UruguayPhoneValidator());

        var user = new User("John", "Doe", "john@example.com", password, phone);

        Assert.AreEqual("John", user.FirstName);
        Assert.AreEqual("Doe", user.LastName);
        Assert.AreEqual("john@example.com", user.Email);
        Assert.AreEqual("ValidPassword1!2$3%", user.Password.Value);
        Assert.AreEqual("099123456", user.Phone.Value);
    }

    [TestMethod]
    public void CreateUser_WhenCreated_HasDefaultCustomerRoleId()
    {
        var user = new User();

        Assert.AreEqual(User.DefaultRoleId, user.RoleId);
    }

    [TestMethod]
    public void CreateUser_UsingConstructor_HasDefaultCustomerRoleId()
    {
        var password = Password.Create("ValidPassword1!2$3%");
        var phone = PhoneNumber.Create("099123456", new UruguayPhoneValidator());

        var user = new User("John", "Doe", "john@example.com", password, phone);

        Assert.AreEqual(User.DefaultRoleId, user.RoleId);
    }

    [TestMethod]
    public void SetRoleId_WithValidRoleId_SetsRoleId()
    {
        var user = new User();

        user.RoleId = 2;

        Assert.AreEqual(2, user.RoleId);
    }

    [TestMethod]
    public void HasPermission_WithMatchingPermission_ReturnsTrue()
    {
        var user = new User();
        var role = new Role(1, "Administrator");
        role.AddPermission(new Permission(1, "CanCreateOrder"));
        user.Role = role;

        var result = user.HasPermission("CanCreateOrder");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void HasPermission_WithoutMatchingPermission_ReturnsFalse()
    {
        var user = new User();
        var role = new Role(1, "Administrator");
        user.Role = role;

        var result = user.HasPermission("CanCreateOrder");

        Assert.IsFalse(result);
    }
}

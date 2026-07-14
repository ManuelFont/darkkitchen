using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.ValueObjects;

namespace DarkKitchen.Domain.Entities;

public sealed class User
{
    public const int DefaultRoleId = 1;

    public Guid Id { get; private set; } = Guid.NewGuid();
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _email = string.Empty;
    private Password? _password;
    private PhoneNumber? _phone;

    public int RoleId { get; set; } = DefaultRoleId;
    public Role Role { get; set; } = null!;

    public User()
    {
    }

    public User(string firstName, string lastName, string email, Password password, PhoneNumber phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Password = password;
        Phone = phone;
    }

    public string FirstName
    {
        get => _firstName;
        set
        {
            ValidateFirstName(value);
            _firstName = value.Trim();
        }
    }

    public string LastName
    {
        get => _lastName;
        set
        {
            ValidateLastName(value);
            _lastName = value.Trim();
        }
    }

    public string Email
    {
        get => _email;
        set
        {
            ValidateEmail(value);
            _email = value.Trim();
        }
    }

    public Password Password
    {
        get => _password!;
        set => _password = value ?? throw new InvalidArgumentException("Password is required");
    }

    public PhoneNumber Phone
    {
        get => _phone!;
        set => _phone = value ?? throw new InvalidArgumentException("Phone is required");
    }

    public bool HasPermission(string permissionName) =>
        Role.RolePermissions.Any(p => p.PermissionName == permissionName);

    private static void ValidateFirstName(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidArgumentException("First name is required");
        }

        if(value.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("First name cannot contain numbers");
        }

        if(value.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
        {
            throw new InvalidArgumentException("First name cannot contain symbols or punctuation");
        }
    }

    private static void ValidateLastName(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidArgumentException("Last name is required");
        }

        if(value.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("Last name cannot contain numbers");
        }

        if(value.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
        {
            throw new InvalidArgumentException("Last name cannot contain symbols or punctuation");
        }

        if(value.Trim().Length < 3 || value.Trim().Length > 25)
        {
            throw new InvalidArgumentException("Last name must be between 3 and 25 characters long");
        }
    }

    private static void ValidateEmail(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidArgumentException("Email is required");
        }

        try
        {
            var address = new System.Net.Mail.MailAddress(value);
            if(address.Address != value)
            {
                throw new InvalidArgumentException("Invalid email format.");
            }
        }
        catch(FormatException)
        {
            throw new InvalidArgumentException("Invalid email format.");
        }
    }
}

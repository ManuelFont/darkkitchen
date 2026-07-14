using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.ValueObjects;

public sealed class Password
{
    public string Value { get; }

    private Password(string value)
    {
        Value = value.Trim();
    }

    public static Password Create(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidArgumentException("Password is required");
        }

        if(value.Length < 15 || value.Length > 25)
        {
            throw new InvalidArgumentException("Password must be between 15 and 25 characters long");
        }

        if(!value.Any(char.IsUpper))
        {
            throw new InvalidArgumentException("Password must contain at least one uppercase letter");
        }

        if(!value.Any(char.IsLower))
        {
            throw new InvalidArgumentException("Password must contain at least one lowercase letter");
        }

        if(!value.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("Password must contain at least one number");
        }

        if(!value.Any(c => char.IsSymbol(c) || char.IsPunctuation(c)))
        {
            throw new InvalidArgumentException("Password must contain at least one special character");
        }

        if(HasSequentialNumbers(value))
        {
            throw new InvalidArgumentException("Password cannot contain sequential numbers");
        }

        return new Password(value);
    }

    public static Password FromStorage(string value)
    {
        return new Password(value);
    }

    private static bool HasSequentialNumbers(string value)
    {
        for(var i = 0; i < value.Length - 2; i++)
        {
            if(char.IsDigit(value[i]) && char.IsDigit(value[i + 1]) && char.IsDigit(value[i + 2]))
            {
                if(value[i + 1] == value[i] + 1 && value[i + 2] == value[i] + 2)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

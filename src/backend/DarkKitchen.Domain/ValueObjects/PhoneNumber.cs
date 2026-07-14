using DarkKitchen.Domain.Exceptions;
using DarkKitchen.Domain.Validators;

namespace DarkKitchen.Domain.ValueObjects;

public sealed class PhoneNumber
{
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public static PhoneNumber Create(string value, IPhoneValidator phoneValidator)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidArgumentException("Phone is required");
        }

        if(!phoneValidator.IsValid(value))
        {
            throw new InvalidArgumentException("Invalid phone number format");
        }

        return new PhoneNumber(value.Replace(" ", string.Empty));
    }

    public static PhoneNumber FromStorage(string value)
    {
        return new PhoneNumber(value);
    }
}

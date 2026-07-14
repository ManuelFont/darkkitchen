namespace DarkKitchen.Domain.ValueObjects;

public sealed record Address
{
    public string Street { get; }
    public int DoorNumber { get; }
    public string? Apartment { get; }

    public Address(string street, int doorNumber, string? apartment)
    {
        ValidateStreet(street);
        ValidateDoorNumber(doorNumber);
        ValidateApartment(apartment);

        Street = street;
        DoorNumber = doorNumber;
        Apartment = apartment;
    }

    private static void ValidateStreet(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Street cannot be null or whitespace.");
        }

        if(value.Any(char.IsSymbol))
        {
            throw new ArgumentException("Street name cannot contain symbols");
        }

        if(value.Any(char.IsPunctuation))
        {
            throw new ArgumentException("Street cannot contain punctuation");
        }
    }

    private static void ValidateDoorNumber(int doorNumber)
    {
        if(doorNumber <= 0)
        {
            throw new ArgumentException("Door number cannot be negative or zero.");
        }
    }

    private static void ValidateApartment(string? value)
    {
        if(value != null && string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Apartment cannot be empty or whitespace.");
        }

        if(value != null && value.Any(char.IsSymbol))
        {
            throw new ArgumentException("Apartment name cannot contain symbols");
        }

        if(value != null && value.Any(char.IsPunctuation))
        {
            throw new ArgumentException("Apartment cannot contain punctuation");
        }
    }
}

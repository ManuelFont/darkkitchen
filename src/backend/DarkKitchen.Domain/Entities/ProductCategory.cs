using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Entities;

public class ProductCategory
{
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    private const int MaxNameLength = 50;
    private const int MaxDescriptionLength = 255;

    public ProductCategory(string name, string description)
    {
        ValidateName(name);
        ValidateDescription(description);

        CategoryId = Guid.NewGuid();
        Name = name.Trim();
        Description = description.Trim();
    }

    public void Update(string name, string description)
    {
        ValidateName(name);
        ValidateDescription(description);
        Name = name.Trim();
        Description = description.Trim();
    }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidArgumentException("Name cannot be empty or whitespace");
        }

        var trimmed = name.Trim();

        if(trimmed.Length > MaxNameLength)
        {
            throw new InvalidArgumentException($"Name cannot exceed {MaxNameLength} characters");
        }

        if(trimmed.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("Name cannot contain numbers");
        }

        if(trimmed.Any(char.IsSymbol))
        {
            throw new InvalidArgumentException("Name cannot contain symbols");
        }

        if(trimmed.Any(char.IsPunctuation))
        {
            throw new InvalidArgumentException("Name cannot contain punctuation");
        }

        if(trimmed.Contains("  "))
        {
            throw new InvalidArgumentException("Name cannot contain consecutive spaces");
        }
    }

    private static void ValidateDescription(string description)
    {
        if(string.IsNullOrWhiteSpace(description))
        {
            throw new InvalidArgumentException("Description cannot be blank or whitespace only");
        }

        var trimmed = description.Trim();

        if(trimmed.Length > MaxDescriptionLength)
        {
            throw new InvalidArgumentException($"Description cannot exceed {MaxDescriptionLength} characters");
        }

        if(trimmed.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("Description cannot contain numbers");
        }

        if(trimmed.Any(char.IsSymbol))
        {
            throw new InvalidArgumentException("Description cannot contain symbols");
        }

        if(trimmed.Any(char.IsControl))
        {
            throw new InvalidArgumentException("Description cannot contain control character");
        }

        if(trimmed.Contains("  "))
        {
            throw new InvalidArgumentException("Description cannot contain consecutive spaces");
        }
    }

    private ProductCategory()
    {
        Name = null!;
        Description = null!;
    }
}

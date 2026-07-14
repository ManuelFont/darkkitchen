using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Entities;

public class Promotion
{
    public Guid Id { get; }
    public string Name { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public Promotion(string name, decimal discountPercentage, DateTime startDate, DateTime endDate)
    {
        ValidateName(name);
        ValidateDiscountPercentage(discountPercentage);
        ValidateDates(startDate, endDate);

        Id = Guid.NewGuid();
        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Update(string name, decimal discountPercentage, DateTime startDate, DateTime endDate)
    {
        ValidateName(name);
        ValidateDiscountPercentage(discountPercentage);
        ValidateDates(startDate, endDate);

        Name = name;
        DiscountPercentage = discountPercentage;
        StartDate = startDate;
        EndDate = endDate;
    }

    public void Update(Promotion promotion)
    {
        ValidateName(promotion.Name);
        ValidateDiscountPercentage(promotion.DiscountPercentage);
        ValidateDates(promotion.StartDate, promotion.EndDate);

        Name = promotion.Name;
        DiscountPercentage = promotion.DiscountPercentage;
        StartDate = promotion.StartDate;
        EndDate = promotion.EndDate;
    }

    public bool IsActive(DateTime date)
    {
        return date >= StartDate && date <= EndDate;
    }

    private static void ValidateName(string name)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidArgumentException("Name cannot be null or empty");
        }

        if(name.Any(char.IsDigit))
        {
            throw new InvalidArgumentException("Name cannot contain number");
        }

        if(name.Any(char.IsPunctuation))
        {
            throw new InvalidArgumentException("Name cannot contain punctuation");
        }

        if(name.Any(char.IsSymbol))
        {
            throw new InvalidArgumentException("Name cannot contain symbol");
        }
    }

    private static void ValidateDiscountPercentage(decimal discountPercentage)
    {
        if(discountPercentage <= 0)
        {
            throw new InvalidArgumentException("Discount cannot be zero or negative");
        }

        if(discountPercentage > 1)
        {
            throw new InvalidArgumentException("Discount must be a fraction between 0 and 1, where 1 represents 100%");
        }
    }

    private static void ValidateDates(DateTime startDate, DateTime endDate)
    {
        if(endDate < startDate)
        {
            throw new InvalidArgumentException("End date cannot be before start date");
        }
    }

    private Promotion()
    {
        Name = null!;
    }
}

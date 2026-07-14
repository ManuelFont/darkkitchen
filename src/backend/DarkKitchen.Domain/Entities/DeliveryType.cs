using DarkKitchen.Domain.Exceptions;

namespace DarkKitchen.Domain.Entities;

public sealed class DeliveryType
{
    public Guid Id { get; private set; }

    public string Name { get; private set; }

    public decimal Cost { get; private set; }

    public DeliveryType(string name, decimal cost)
    {
        Validate(name, cost);

        Id = Guid.NewGuid();
        Name = name;
        Cost = cost;
    }

    public void Update(string name, decimal cost)
    {
        Validate(name, cost);

        Name = name;
        Cost = cost;
    }

    private static void Validate(string name, decimal cost)
    {
        if(string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidArgumentException("Delivery type name is required");
        }

        if(cost < 0)
        {
            throw new InvalidArgumentException("Delivery type cost cannot be negative");
        }
    }

    private DeliveryType()
    {
        Name = string.Empty;
    }
}

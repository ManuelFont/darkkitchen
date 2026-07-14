namespace DarkKitchen.Domain.Exceptions;

public sealed class ResourceNotFoundException : DomainException
{
    public string ResourceName { get; }
    public string Identifier { get; }

    public ResourceNotFoundException(string resourceName, Guid id)
        : base($"Resource '{resourceName}' with id '{id}' was not found.")
    {
        ResourceName = resourceName;
        Identifier = id.ToString();
    }

    public ResourceNotFoundException(string resourceName, int id)
        : base($"Resource '{resourceName}' with id '{id}' was not found.")
    {
        ResourceName = resourceName;
        Identifier = id.ToString();
    }

    public ResourceNotFoundException(string resourceName, string identifier)
        : base($"Resource '{resourceName}' with identifier '{identifier}' was not found.")
    {
        ResourceName = resourceName;
        Identifier = identifier;
    }
}

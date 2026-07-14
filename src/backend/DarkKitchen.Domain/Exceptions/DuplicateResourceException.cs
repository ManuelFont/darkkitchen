namespace DarkKitchen.Domain.Exceptions;

public sealed class DuplicateResourceException(string resourceName, string conflictingField, string value)
    : DomainException($"Resource '{resourceName}' with {conflictingField} '{value}' already exists.")
{
    public string ResourceName { get; } = resourceName;
    public string ConflictingField { get; } = conflictingField;
    public string Value { get; } = value;
}

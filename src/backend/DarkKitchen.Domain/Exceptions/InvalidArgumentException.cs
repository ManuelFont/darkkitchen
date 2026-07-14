namespace DarkKitchen.Domain.Exceptions;

public sealed class InvalidArgumentException(string message) : DomainException(message)
{
}

namespace DarkKitchen.Domain.Exceptions;

public sealed class TokenExpiredException(string message) : DomainException(message)
{
}

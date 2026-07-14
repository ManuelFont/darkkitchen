namespace DarkKitchen.Application.Abstractions;

public interface ICurrentUserContext
{
    bool IsAuthenticated { get; }
    Guid UserId { get; }
    string UserEmail { get; }
}

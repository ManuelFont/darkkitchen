namespace DarkKitchen.WebApi.Responses.Users;

public readonly struct CreateUserResponse(Guid id)
{
    public Guid Id { get; } = id;
}

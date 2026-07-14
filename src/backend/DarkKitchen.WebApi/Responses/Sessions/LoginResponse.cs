namespace DarkKitchen.WebApi.Responses.Sessions;

public readonly struct LoginResponse(Guid token, string role)
{
    public Guid Token { get; } = token;
    public string Role { get; } = role;
}

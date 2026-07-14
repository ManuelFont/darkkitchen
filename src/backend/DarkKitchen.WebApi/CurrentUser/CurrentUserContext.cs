using DarkKitchen.Application.Abstractions;
using DarkKitchen.Domain.Entities;
using DarkKitchen.WebApi.Filters;

namespace DarkKitchen.WebApi.CurrentUser;

internal sealed class CurrentUserContext(IHttpContextAccessor accessor) : ICurrentUserContext
{
    private User? User => accessor.HttpContext?.Items[HttpContextItemKey.UserLogged] as User;

    public bool IsAuthenticated => User is not null;
    public Guid UserId => User?.Id ?? Guid.Empty;
    public string UserEmail => User?.Email ?? string.Empty;
}

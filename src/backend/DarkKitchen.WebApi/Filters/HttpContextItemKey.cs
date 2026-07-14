namespace DarkKitchen.WebApi.Filters;

/// <summary>Keys used to store request-scoped data in HttpContext.Items.</summary>
internal enum HttpContextItemKey
{
    /// <summary>The authenticated user resolved from the session token.</summary>
    UserLogged
}

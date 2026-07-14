using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
internal sealed class AuthorizationFilter(PermissionNames permission) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(context.Result != null)
        {
            return;
        }

        var userLogged = context.HttpContext.Items[HttpContextItemKey.UserLogged];

        if(userLogged is not Domain.Entities.User user)
        {
            context.Result = new ObjectResult(new ErrorResponse("UnAuthorized", "Not authenticated"))
            {
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
            return;
        }

        if(!user.HasPermission(permission.ToString()))
        {
            context.Result = new ObjectResult(new ErrorResponse("Forbidden", $"Missing permission {permission}"))
            {
                StatusCode = (int)HttpStatusCode.Forbidden
            };
        }
    }
}

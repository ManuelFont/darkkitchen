using System.Net;
using DarkKitchen.Application.Services.Sessions;
using DarkKitchen.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

internal sealed class AuthenticationFilter(ISessionService sessionService) : IAuthorizationFilter
{
    private readonly ISessionService _sessionService = sessionService;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorization = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();

        if(string.IsNullOrWhiteSpace(authorization))
        {
            context.Result = new ObjectResult(new ErrorResponse("UnAuthorized", "Not authenticated"))
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
            return;
        }

        if(!Guid.TryParse(authorization, out var token))
        {
            context.Result = new ObjectResult(new ErrorResponse("UnAuthorized", "Token invalid"))
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
            return;
        }

        try
        {
            var user = _sessionService.GetUserByToken(token);
            context.HttpContext.Items[HttpContextItemKey.UserLogged] = user;
        }
        catch(TokenExpiredException)
        {
            context.Result = new ObjectResult(new ErrorResponse("UnAuthorized", "Token expired"))
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
        }
        catch(Exception)
        {
            context.Result = new ObjectResult(new ErrorResponse("UnAuthorized", "Token does not exist"))
            {
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
        }
    }
}

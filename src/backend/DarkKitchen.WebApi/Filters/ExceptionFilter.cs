using System.Net;
using DarkKitchen.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DarkKitchen.WebApi.Filters;

internal sealed class ExceptionFilter(IWebHostEnvironment environment) : IExceptionFilter
{
    private readonly IWebHostEnvironment _environment = environment;

    private readonly Dictionary<Type, Func<Exception, IActionResult>> _errors = new()
    {
        {
            typeof(InvalidArgumentException),
            ex => new ObjectResult(new ErrorResponse("InvalidArgument", ex.Message))
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            }
        },
        {
            typeof(ResourceNotFoundException),
            ex => new ObjectResult(new ErrorResponse("ResourceNotFound", ex.Message))
            {
                StatusCode = (int)HttpStatusCode.NotFound
            }
        },
        {
            typeof(DuplicateResourceException),
            ex => new ObjectResult(new ErrorResponse("DuplicateResource", ex.Message))
            {
                StatusCode = (int)HttpStatusCode.Conflict
            }
        }
    };

    public void OnException(ExceptionContext context)
    {
        var factory = _errors.GetValueOrDefault(context.Exception.GetType());

        if(factory == null)
        {
            var message = "There was an error when processing the request";
            if(_environment.IsDevelopment())
            {
                message = context.Exception.Message;
            }

            context.Result = new ObjectResult(new ErrorResponse("InternalError", message))
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
        else
        {
            context.Result = factory(context.Exception);
        }

        context.ExceptionHandled = true;
    }
}

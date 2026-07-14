using Microsoft.AspNetCore.Mvc;

namespace DarkKitchen.WebApi.Filters;

internal static class ModelStateResponseFactory
{
    internal static IActionResult Handle(ActionContext context)
    {
        var firstError = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault() ?? "Invalid request";

        return new BadRequestObjectResult(new ErrorResponse("InvalidArgument", firstError));
    }
}

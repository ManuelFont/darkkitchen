using System.Diagnostics.CodeAnalysis;
using Scalar.AspNetCore;

namespace DarkKitchen.WebApi.Configuration;

[ExcludeFromCodeCoverage]
internal sealed class ScalarConfig
{
    public void Config(WebApplication app)
    {
        if(app.Environment.IsDevelopment())
        {
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "openapi/{documentName}.json";
            });
            app.MapScalarApiReference();
        }
    }
}

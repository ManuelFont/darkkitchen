using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.WebApi.Configuration;

[ExcludeFromCodeCoverage]
internal sealed class CorsConfig
{
    private const string AngularCorsPolicy = "AngularCorsPolicy";

    public void AddPolicies(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AngularCorsPolicy, policy =>
            {
                policy.WithOrigins(GetAllowedOrigins())
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public void UsePolicies(WebApplication app)
    {
        app.UseCors(AngularCorsPolicy);

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SqlDbContext>();
        dbContext.Database.Migrate();
    }

    private static string[] GetAllowedOrigins()
    {
        var configuredOrigin = Environment.GetEnvironmentVariable("Cors__AllowedOrigin");

        return string.IsNullOrWhiteSpace(configuredOrigin)
            ? ["http://localhost:4200", "http://localhost:8080", "http://127.0.0.1:4200"]
            : [configuredOrigin];
    }
}

using System.Diagnostics.CodeAnalysis;
using DarkKitchen.Application.Abstractions;
using DarkKitchen.Application.Services.Audit;
using DarkKitchen.Domain.Repositories.EntityRepositories;
using DarkKitchen.Infrastructure.Repositories;
using DarkKitchen.WebApi.CurrentUser;

namespace DarkKitchen.WebApi.Configuration;

[ExcludeFromCodeCoverage]
internal sealed class AuditConfig
{
    public void AddServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAuditService, AuditService>();
    }
}

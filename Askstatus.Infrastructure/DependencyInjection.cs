using System.Reflection;
using Askstatus.Application.Interfaces;
using Askstatus.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Askstatus.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
}

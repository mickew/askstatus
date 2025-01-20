using System.Reflection;
using Askstatus.Application.Authorization;
using Askstatus.Application.Interfaces;
using Askstatus.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Askstatus.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddHttpClient();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddSingleton<IAuthorizationPolicyProvider, FlexibleAuthorizationPolicyProvider>();
        services.AddScoped<IDiscoverDeviceService, ShellyDiscoverDeviceService>();
        services.AddScoped<IDeviceService, Shelly2DeviceService>();
        return services;
    }
}

using Askstatus.Application.Interfaces;
using Askstatus.Infrastructure.Authorization;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using Askstatus.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Askstatus.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Establish cookie authentication
        services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies(); // (o => o.ApplicationCookie!.Configure(s => s.LoginPath = "/Identity/Account/Login"));

        // Configure authorization
        services.AddAuthorizationBuilder();

        // Add the database (in memory for the sample)
        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseInMemoryDatabase("AppDb");
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                //For debugging only: options.EnableDetailedErrors(true);
                //For debugging only: options.EnableSensitiveDataLogging(true);
            });

        // Add identity and opt-in to endpoints
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddApiEndpoints();
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
}

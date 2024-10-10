using Askstatus.Application.Interfaces;
using Askstatus.Infrastructure.Authorization;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using Askstatus.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Askstatus.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IWebHostEnvironment environment, string connectionString = "Data Source=db.db")
    {
        var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
        if (!Path.IsPathRooted(sqliteBuilder.DataSource))
        {
            sqliteBuilder.DataSource = Path.Combine($"{environment.ContentRootPath}{Path.DirectorySeparatorChar}", sqliteBuilder.DataSource);
        }

        // Establish cookie authentication
        services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies(); // (o => o.ApplicationCookie!.Configure(s => s.LoginPath = "/Identity/Account/Login"));

        // Configure authorization
        services.AddAuthorizationBuilder();

        // Add the database
        services.AddDbContext<ApplicationBaseDbContext>(options =>
        {
            options.UseSqlite(sqliteBuilder.ToString());
        });

        // Add identity and opt-in to endpoints
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationBaseDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddApiEndpoints();

        ///////////////////////////////////////////////
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IApplicationHostAddressService, ApplicationHostAddressService>();

        return services;
    }
}

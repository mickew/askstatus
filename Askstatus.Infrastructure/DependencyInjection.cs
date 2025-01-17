using System.Reflection;
using Askstatus.Application.Interfaces;
using Askstatus.Infrastructure.Authorization;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using Askstatus.Infrastructure.Services;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Askstatus.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IWebHostEnvironment environment, string connectionString = "Data Source=db.db")
    {
        ArgumentNullException.ThrowIfNull(services);
        var sqliteBuilder = new SqliteConnectionStringBuilder(connectionString);
        if (!Path.IsPathRooted(sqliteBuilder.DataSource))
        {
            sqliteBuilder.DataSource = Path.Combine($"{environment.ContentRootPath}{Path.DirectorySeparatorChar}", sqliteBuilder.DataSource);
        }

        // Establish cookie authentication
        services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies(o =>
        {
            o.ApplicationCookie!.Configure(s =>
            {
                s.Events.OnRedirectToAccessDenied =
                s.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                s.ExpireTimeSpan = TimeSpan.FromHours(1);
            });
        });

        // Configure authorization
        //services.AddAuthorizationBuilder();
        services.AddAuthorization();


        // Add the database
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(sqliteBuilder.ToString());
        });

        // Add identity and opt-in to endpoints
        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddApiEndpoints();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //cfg.NotificationPublisher = new ForeachAwaitPublisher();
            cfg.NotificationPublisher = new TaskWhenAllPublisher();
        });

        services.AddSignalR();

        ///////////////////////////////////////////////
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserService, UserService>();
        services.AddSingleton<IApplicationHostAddressService, ApplicationHostAddressService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IRepository<Askstatus.Domain.Entities.PowerDevice>, Repository<Askstatus.Domain.Entities.PowerDevice>>();

        return services;
    }
}

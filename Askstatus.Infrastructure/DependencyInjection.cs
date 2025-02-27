using System.Reflection;
using Askstatus.Application.Events;
using Askstatus.Application.Interfaces;
using Askstatus.Domain;
using Askstatus.Infrastructure.Authorization;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Events;
using Askstatus.Infrastructure.Identity;
using Askstatus.Infrastructure.Mail;
using Askstatus.Infrastructure.Services;
using MediatR.NotificationPublishers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Askstatus.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IWebHostEnvironment environment, string connectionString = "Data Source=db.db")
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<MailSettings>()
            .BindConfiguration(MailSettings.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<AskstatusApiSettings>()
            .BindConfiguration(AskstatusApiSettings.Section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

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
        services.AddIdentityCore<ApplicationUser>(opt =>
            {
                //opt.Tokens.EmailConfirmationTokenProvider = "Email";
                opt.SignIn.RequireConfirmedEmail = true;
                opt.User.RequireUniqueEmail = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequiredLength = 8;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                opt.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            .AddApiEndpoints();

        services.AddMediatR(cfg =>
        {
            var ass = Assembly.GetExecutingAssembly();
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //cfg.NotificationPublisher = new ForeachAwaitPublisher();
            cfg.NotificationPublisher = new TaskWhenAllPublisher();
        });

        services.AddSignalR();

        ///////////////////////////////////////////////
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAskStatusSmtpClient, MailKitSmtpClient>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileService, FileService>();

        services.AddSingleton<InMemoryMessageQueue>();
        services.AddSingleton<IEventBus, EventBus>();
        services.AddHostedService<IntegrationEventProcessorJob>();

        services.AddSingleton<IApplicationHostAddressService, ApplicationHostAddressService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IRepository<Askstatus.Domain.Entities.PowerDevice>, Repository<Askstatus.Domain.Entities.PowerDevice>>();
        services.AddTransient<IRepository<Askstatus.Domain.Entities.SystemLog>, Repository<Askstatus.Domain.Entities.SystemLog>>();

        services.AddSingleton<IMqttClientService, MqttClientService>();
        services.AddSingleton<IHostedService>(serviceProvider =>
        {
            return serviceProvider.GetService<IMqttClientService>()!;
        });

        return services;
    }
}

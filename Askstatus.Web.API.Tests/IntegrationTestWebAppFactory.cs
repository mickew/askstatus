﻿using Askstatus.Common.Authorization;
using Askstatus.Common.PowerDevice;
using Askstatus.Domain.Entities;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Papercut;

namespace Askstatus.Web.API.Tests;
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public PapercutContainer PapercutContainer { get; private set; }

    public const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";
    public const string UserRole = "Users";
    public const string DefaultUserUserName = "user";

    public const string DefaultPassword = "!PassW0rd!";

    public string? AdminId { get; private set; }

    public string? UserId { get; private set; }

    public string? AdministratorsRoleId { get; private set; }

    public string? UserRoleId { get; private set; }

    public int PowerDeviceId { get; private set; }

    public IntegrationTestWebAppFactory()
    {
        PapercutContainer = new PapercutBuilder().Build();
    }
    public async Task InitializeAsync()
    {
        Program.IsIntegrationTestRun = true;
        await PapercutContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await PapercutContainer.DisposeAsync().AsTask();
    }

    public Task SetUsersPermission(Permissions permission)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var role = context.Roles.FirstOrDefault(r => r.Name == UserRole);
        if (role != null)
        {
            role.Permissions = permission;
            context.SaveChanges();
        }
        return Task.CompletedTask;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {
            // Remove the existing service registration for the DbContext
            RemoveAllDbContextsFromServices(services);

            // Add a database context using an in-memory database for testing.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                //var connectionString = new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString();
                //var connection = new SqliteConnection(connectionString);
                //options.UseSqlite(connection);
                //options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                options.UseInMemoryDatabase("TestDb");
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
            });
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //db.Database.EnsureCreated();
                SeedData(db);
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                if (config is IConfigurationRoot root)
                {
                    root["MailSettings:Host"] = PapercutContainer.Hostname;
                    root["MailSettings:Port"] = PapercutContainer.GetMappedPublicPort(25).ToString();
                    root.Reload();
                }
            };
        });

    }


    public void ReSeedData()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        UnSeedData(context);
        SeedData(context);
    }

    private void UnSeedData(ApplicationDbContext context)
    {
        context.UserRoles.RemoveRange(context.UserRoles);
        context.Users.RemoveRange(context.Users);
        context.Roles.RemoveRange(context.Roles);
        context.PowerDevices.RemoveRange(context.PowerDevices);
        context.SaveChanges();
    }

    private void RemoveAllDbContextsFromServices(IServiceCollection services)
    {
        // reverse operation of AddDbContext<XDbContext> which removes  DbContexts from services
        var descriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContextOptions)).ToList();
        descriptors.ForEach(d => services.Remove(d));

        var dbContextDescriptors = services.Where(d => d.ServiceType.BaseType == typeof(DbContext)).ToList();
        dbContextDescriptors.ForEach(d => services.Remove(d));
    }

    private void SeedData(ApplicationDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }
        ApplicationRole adminRole = new()
        {
            Name = AdministratorsRole,
            NormalizedName = AdministratorsRole.ToUpper(),
            Permissions = Permissions.All
        };
        ApplicationRole userRole = new()
        {
            Name = UserRole,
            NormalizedName = UserRole.ToUpper(),
            Permissions = Permissions.None
        };

        if (!context.Roles.Any())
        {
            context.Roles.Add(adminRole);
            AdministratorsRoleId = adminRole.Id;
            context.Roles.Add(userRole);
            UserRoleId = userRole.Id;
        }

        // Create default admin user
        var adminUserName = DefaultAdminUserName;
        var adminUser = new ApplicationUser()
        {
            UserName = adminUserName,
            Email = "admin@localhost.local",
            NormalizedUserName = adminUserName.ToUpper(),
            NormalizedEmail = "admin@localhost.local".ToUpper(),
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true,
            LockoutEnabled = false
        };
        PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
        var pw = passwordHasher.HashPassword(adminUser, DefaultPassword);
        adminUser.PasswordHash = pw;

        // Create default user user
        var userUserName = DefaultUserUserName;
        var userUser = new ApplicationUser()
        {
            UserName = userUserName,
            Email = "user@localhost.local",
            NormalizedUserName = userUserName.ToUpper(),
            NormalizedEmail = "user@localhost.local".ToUpper(),
            FirstName = "User",
            LastName = "User",
            EmailConfirmed = true,
            LockoutEnabled = false
        };
        pw = passwordHasher.HashPassword(userUser, DefaultPassword);
        userUser.PasswordHash = pw;

        if (!context.Users.Any())
        {
            var admn = context.Users.Add(adminUser);
            AdminId = admn.Entity.Id;
            context.Users.Add(userUser);
            UserId = userUser.Id;
            context.Add(new IdentityUserRole<string>() { RoleId = adminRole.Id, UserId = adminUser.Id });
            context.Add(new IdentityUserRole<string>() { RoleId = userRole.Id, UserId = userUser.Id });
        }

        if (!context.PowerDevices.Any())
        {
            var powdev = context.PowerDevices.Add(new PowerDevice()
            {
                Name = "Test Device",
                DeviceType = PowerDeviceTypes.ShellyGen2,
                HostName = "192.168.1.85",
                DeviceName = "Test Device",
                DeviceId = "Test Device",
                DeviceMac = "EC626081CDF4",
                DeviceModel = "Test Model",
                Channel = 0
            });
            PowerDeviceId = powdev.Entity.Id;
        }
        context.SaveChanges();
    }
}

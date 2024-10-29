using Askstatus.Common.Authorization;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Askstatus.Web.API.Tests;
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";
    public const string UserRole = "Users";
    public const string DefaultUserUserName = "user";

    public const string DefaultPassword = "!PassW0rd!";

    public string? AdminId { get; private set; }

    public string? UserId { get; private set; }

    public string? AdministratorsRoleId { get; private set; }

    public string? UserRoleId { get; private set; }

    public Task InitializeAsync()
    {
        Program.IsIntegrationTestRun = true;
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public Task SetUsersPermission(Permissions permission)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationBaseDbContext>();
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
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationBaseDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context using an in-memory database for testing.
            services.AddDbContext<ApplicationBaseDbContext>(options =>
            {
                //options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                options.UseInMemoryDatabase("TestDb");
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
            });
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationBaseDbContext>();
                SeedData(db);
            };
        });
    }

    public void ReSeedData()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationBaseDbContext>();
        UnSeedData(context);
        SeedData(context);
    }

    private void UnSeedData(ApplicationBaseDbContext context)
    {
        context.UserRoles.RemoveRange(context.UserRoles);
        context.Users.RemoveRange(context.Users);
        context.Roles.RemoveRange(context.Roles);
        context.SaveChanges();
    }

    private void SeedData(ApplicationBaseDbContext context)
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
            LastName = "User"
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
            LastName = "User"
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
        context.SaveChanges();
    }
}

using Askstatus.Domain.Authorization;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Askstatus.Web.API.Tests;
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";

    public const string DefaultPassword = "Password123!";

    //private ApplicationDbContext? _context;

    public Task InitializeAsync()
    {
        Program.IsIntegrationTestRun = true;
        return Task.CompletedTask;
    }

    public new Task DisposeAsync()
    {
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

        if (!context.Roles.Any())
        {
            context.Roles.Add(adminRole);
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
        if (!context.Users.Any())
        {
            context.Users.Add(adminUser);
            context.Add(new IdentityUserRole<string>() { RoleId = adminRole.Id, UserId = adminUser.Id });
        }
        context.SaveChanges();
    }
}

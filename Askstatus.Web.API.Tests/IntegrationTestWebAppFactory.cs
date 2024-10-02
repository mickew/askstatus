using Askstatus.Domain.Authorization;
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
    private const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";

    public const string DefaultPassword = "Password123!";

    private ApplicationDbContext? _context;

    public async Task InitializeAsync()
    {
        _context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options);
        await SeedDataAsync();
    }

    public new Task DisposeAsync()
    {
        return _context!.DisposeAsync().AsTask();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.ConfigureServices(services =>
        {

            // Remove the existing service registration for the DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add a database context using an in-memory database for testing.
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
                options.EnableDetailedErrors(true);
                options.EnableSensitiveDataLogging(true);
                //For debugging only: options.EnableDetailedErrors(true);
                //For debugging only: options.EnableSensitiveDataLogging(true);
            });
        });
    }

    private async Task SeedDataAsync()
    {
        await _context!.Database.EnsureCreatedAsync();
        if (_context.Users.Any())
        {
            return;
        }
        ApplicationRole adminRole = new()
        {
            Name = AdministratorsRole,
            NormalizedName = AdministratorsRole.ToUpper(),
            Permissions = Permissions.All
        };

        if (!await _context.Roles.AnyAsync())
        {
            await _context.Roles.AddAsync(adminRole);
        }

        // Create default admin user
        var adminUserName = DefaultAdminUserName;
        var adminUser = new ApplicationUser()
        {
            UserName = adminUserName,
            Email = "admin@localhost.local",
            NormalizedUserName = adminUserName.ToUpper(),
            NormalizedEmail = "admin@localhost.local".ToUpper(),
        };
        PasswordHasher<ApplicationUser> passwordHasher = new PasswordHasher<ApplicationUser>();
        var pw = passwordHasher.HashPassword(adminUser, DefaultPassword);
        adminUser.PasswordHash = pw;
        if (!await _context.Users.AnyAsync())
        {
            await _context.Users.AddAsync(adminUser);
            await _context.AddAsync(new IdentityUserRole<string>() { RoleId = adminRole.Id, UserId = adminUser.Id });
        }
        await _context.SaveChangesAsync();
    }
}

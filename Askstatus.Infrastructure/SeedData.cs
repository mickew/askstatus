using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Askstatus.Domain.Authorization;
using Askstatus.Infrastructure.Data;
using Askstatus.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Askstatus.Infrastructure;
public class SeedData
{
    private const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";

    private const string DefaultPassword = "Password123!";

    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationBaseDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationBaseDbContext>>());

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

        if (!await context.Roles.AnyAsync())
        {
            await context.Roles.AddAsync(adminRole);
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
        if (!await context.Users.AnyAsync())
        {
            await context.Users.AddAsync(adminUser);
            await context.AddAsync(new IdentityUserRole<string>() { RoleId = adminRole.Id, UserId = adminUser.Id });
        }
        await context.SaveChangesAsync();
    }
}

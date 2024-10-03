using Askstatus.Domain.Authorization;
using Askstatus.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Askstatus.Infrastructure.Data;

public sealed class DbInitializer
{
    private readonly ApplicationBaseDbContext _context;

    private const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";

    private const string DefaultPassword = "Password123!";

    public DbInitializer(ApplicationBaseDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        _context.Database.Migrate();

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
            FirstName = "Admin",
            LastName = "User"
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

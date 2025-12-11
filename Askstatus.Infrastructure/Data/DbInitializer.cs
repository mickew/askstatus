using Askstatus.Common.Authorization;
using Askstatus.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Askstatus.Infrastructure.Data;

public sealed class DbInitializer
{
    private readonly ApplicationDbContext _context;

    public const string AdministratorsRole = "Administrators";
    public const string DefaultAdminUserName = "admin";
    private const string UserRole = "Users";
    public const string DefaultUserUserName = "user";

    private const string DefaultPassword = "Password123!";

    public DbInitializer(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(string seedPassword)
    {
        _context.Database.Migrate();

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

        if (!await _context.Roles.AnyAsync())
        {
            await _context.Roles.AddAsync(adminRole);
            await _context.Roles.AddAsync(userRole);
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


        if (!await _context.Users.AnyAsync())
        {
            await _context.Users.AddAsync(adminUser);
            await _context.Users.AddAsync(userUser);
            await _context.AddAsync(new IdentityUserRole<string>() { RoleId = adminRole.Id, UserId = adminUser.Id });
            await _context.AddAsync(new IdentityUserRole<string>() { RoleId = userRole.Id, UserId = userUser.Id });
        }

        if (!string.IsNullOrEmpty(seedPassword))
        {
            adminUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == adminUserName);
            if (adminUser is not null)
            {
                pw = passwordHasher.HashPassword(adminUser, seedPassword);
                adminUser.PasswordHash = pw;
                _context.Users.Update(adminUser);
            }
        }
        adminUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == adminUserName);
        if (adminUser is not null)
        {
            adminUser.EmailConfirmed = true;
            adminUser.LockoutEnabled = false;
            _context.Users.Update(adminUser);
        }
        userUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userUserName);
        if (userUser is not null)
        {
            userUser.EmailConfirmed = true;
            userUser.LockoutEnabled = false;
            _context.Users.Update(userUser);
        }
        await _context.SaveChangesAsync();
    }
}

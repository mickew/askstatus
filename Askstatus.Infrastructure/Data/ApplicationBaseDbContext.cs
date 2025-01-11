using System.Reflection;
using Askstatus.Domain.Entities;
using Askstatus.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Askstatus.Infrastructure.Data;
public class ApplicationBaseDbContext<TPowerDevice> : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    where TPowerDevice : PowerDevice
{
    public ApplicationBaseDbContext()
    {

    }
    public ApplicationBaseDbContext(DbContextOptions options)
        : base(options)
    {

    }

    public DbSet<TPowerDevice> PowerDevices { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : ApplicationBaseDbContext<PowerDevice>(options)
{
}

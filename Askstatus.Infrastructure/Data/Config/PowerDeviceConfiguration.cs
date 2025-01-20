using Askstatus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Askstatus.Infrastructure.Data.Config;
internal class PowerDeviceConfiguration : IEntityTypeConfiguration<PowerDevice>
{
    public void Configure(EntityTypeBuilder<PowerDevice> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.DeviceType).IsRequired();
        builder.Property(p => p.HostName).IsRequired();
        builder.Property(p => p.DeviceName).IsRequired();
        builder.Property(p => p.DeviceId).IsRequired();
        builder.Property(p => p.DeviceMac).IsRequired();
        builder.Property(p => p.DeviceModel).IsRequired();
        builder.Property(p => p.Channel).IsRequired();

        builder.HasIndex(p => new {p.DeviceId, p.Channel}).IsUnique();
        builder.HasIndex(p => new { p.DeviceMac, p.Channel }).IsUnique();
        builder.HasIndex(p => new {p.HostName, p.Channel }).IsUnique();
    }
}

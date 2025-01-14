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
        builder.HasIndex(p => p.HostName).IsUnique();
        builder.Property(p => p.DeviceName).IsRequired();
        builder.Property(p => p.DeviceId).IsRequired();
        builder.Property(p => p.DeviceMac).IsRequired();
        builder.HasIndex(p => p.DeviceMac).IsUnique();
        builder.Property(p => p.DeviceModel).IsRequired();
        builder.Property(p => p.DeviceGen).IsRequired();
    }
}

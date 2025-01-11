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
    }
}

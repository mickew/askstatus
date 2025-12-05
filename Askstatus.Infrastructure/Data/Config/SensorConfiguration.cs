using Askstatus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Askstatus.Infrastructure.Data.Config;
internal class SensorConfiguration : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedOnAdd();
        builder.Property(s => s.Name).IsRequired();
        builder.Property(s => s.SensorType).IsRequired();
        builder.Property(s => s.SensorName).IsRequired();
        builder.Property(s => s.ValueName).IsRequired();

        builder.HasIndex(s => new { s.SensorName, s.ValueName }).IsUnique();
    }
}

using Askstatus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Askstatus.Infrastructure.Data.Config;
internal class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
{
    public void Configure(EntityTypeBuilder<SystemLog> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.Property(p => p.EventTime).IsRequired();
        builder.Property(p => p.EventType).IsRequired();
        builder.Property(p => p.User).IsRequired();
        builder.Property(p => p.Message).IsRequired();
    }
}

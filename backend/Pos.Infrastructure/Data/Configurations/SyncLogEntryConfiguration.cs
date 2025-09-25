using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;
using Pos.Domain.Enums;

namespace Pos.Infrastructure.Data.Configurations;

internal class SyncLogEntryConfiguration : IEntityTypeConfiguration<SyncLogEntry>
{
    public void Configure(EntityTypeBuilder<SyncLogEntry> builder)
    {
        builder.ToTable("sync_log");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id");
        builder.Property(s => s.EntityName).HasColumnName("entity_name");
        builder.Property(s => s.EntityId).HasColumnName("entity_id");
        builder.Property(s => s.Operation).HasColumnName("operation").HasConversion(v => v.ToString().ToLowerInvariant(), v => Enum.Parse<SyncOperationType>(v, true));
        builder.Property(s => s.Origin).HasColumnName("origin");
        builder.Property(s => s.Payload).HasColumnName("payload");
        builder.Property(s => s.Processed).HasColumnName("processed");
        builder.Property(s => s.ErrorMessage).HasColumnName("error_message");
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
    }
}

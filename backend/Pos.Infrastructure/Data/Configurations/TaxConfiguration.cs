using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class TaxConfiguration : IEntityTypeConfiguration<Tax>
{
    public void Configure(EntityTypeBuilder<Tax> builder)
    {
        builder.ToTable("catalog_taxes");
        builder.ConfigureAuditableEntity();
        builder.Property(t => t.Code).HasColumnName("code").IsRequired();
        builder.Property(t => t.Name).HasColumnName("name").IsRequired();
        builder.Property(t => t.Rate).HasColumnName("rate").HasColumnType("numeric(5,2)");
        builder.Property(t => t.Scope).HasColumnName("scope").IsRequired();
        builder.Property(t => t.IsCompound).HasColumnName("is_compound");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class PriceListConfiguration : IEntityTypeConfiguration<PriceList>
{
    public void Configure(EntityTypeBuilder<PriceList> builder)
    {
        builder.ToTable("catalog_price_lists");
        builder.ConfigureAuditableEntity();
        builder.Property(pl => pl.Code).HasColumnName("code").IsRequired();
        builder.Property(pl => pl.Name).HasColumnName("name").IsRequired();
        builder.Property(pl => pl.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(pl => pl.IsDefault).HasColumnName("is_default");
        builder.Property(pl => pl.ValidFrom).HasColumnName("valid_from");
        builder.Property(pl => pl.ValidUntil).HasColumnName("valid_until");
        builder.HasMany(pl => pl.Items).WithOne().HasForeignKey(i => i.PriceListId);
        builder.Navigation(pl => pl.Items).HasField("_items");
        builder.Navigation(pl => pl.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

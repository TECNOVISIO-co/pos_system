using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class PriceListItemConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> builder)
    {
        builder.ToTable("catalog_price_list_items");
        builder.ConfigureAuditableEntity();
        builder.Property(i => i.PriceListId).HasColumnName("price_list_id");
        builder.Property(i => i.ProductId).HasColumnName("product_id");
        builder.Property(i => i.Price).HasColumnName("price").HasColumnType("numeric(18,2)");
        builder.Property(i => i.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(i => i.ValidFrom).HasColumnName("valid_from");
        builder.Property(i => i.ValidUntil).HasColumnName("valid_until");
        builder.HasIndex(i => new { i.PriceListId, i.ProductId }).IsUnique();
    }
}

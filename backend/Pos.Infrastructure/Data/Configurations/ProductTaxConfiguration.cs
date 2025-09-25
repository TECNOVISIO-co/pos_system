using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class ProductTaxConfiguration : IEntityTypeConfiguration<ProductTax>
{
    public void Configure(EntityTypeBuilder<ProductTax> builder)
    {
        builder.ToTable("catalog_product_taxes");
        builder.HasKey(pt => new { pt.ProductId, pt.TaxId });
        builder.Property(pt => pt.ProductId).HasColumnName("product_id");
        builder.Property(pt => pt.TaxId).HasColumnName("tax_id");
        builder.Property(pt => pt.Priority).HasColumnName("priority");
        builder.Property<DateTimeOffset>("created_at");
        builder.Property<Guid?>("created_by");
    }
}

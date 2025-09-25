using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("catalog_products");
        builder.ConfigureAuditableEntity();
        builder.Property(p => p.Sku).HasColumnName("sku").IsRequired();
        builder.Property(p => p.Name).HasColumnName("name").IsRequired();
        builder.Property(p => p.Description).HasColumnName("description");
        builder.Property(p => p.Barcode).HasColumnName("barcode");
        builder.Property(p => p.Brand).HasColumnName("brand");
        builder.Property(p => p.Model).HasColumnName("model");
        builder.Property(p => p.UnitOfMeasure).HasColumnName("unit_of_measure").IsRequired();
        builder.Property(p => p.Cost).HasColumnName("cost").HasColumnType("numeric(18,2)");
        builder.Property(p => p.Price).HasColumnName("price").HasColumnType("numeric(18,2)");
        builder.Property(p => p.IsActive).HasColumnName("is_active");
        builder.Property(p => p.MinStock).HasColumnName("min_stock").HasColumnType("numeric(12,3)");
        builder.Property(p => p.AllowNegativeStock).HasColumnName("allow_negative_stock");
        builder.Property(p => p.TaxRuleId).HasColumnName("tax_rule_id");
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.HasMany(p => p.Taxes).WithOne().HasForeignKey(pt => pt.ProductId);
        builder.Navigation(p => p.Taxes).HasField("_taxes");
        builder.Navigation(p => p.Taxes).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

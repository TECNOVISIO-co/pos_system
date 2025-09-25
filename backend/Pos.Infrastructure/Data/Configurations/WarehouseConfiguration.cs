using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("inventory_warehouses");
        builder.ConfigureAuditableEntity();
        builder.Property(w => w.Code).HasColumnName("code").IsRequired();
        builder.Property(w => w.Name).HasColumnName("name").IsRequired();
        builder.Property(w => w.Location).HasColumnName("location");
        builder.Property(w => w.IsDefault).HasColumnName("is_default");
        builder.Property(w => w.AllowSales).HasColumnName("allow_sales");
    }
}

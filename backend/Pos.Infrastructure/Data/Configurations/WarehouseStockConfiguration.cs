using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class WarehouseStockConfiguration : IEntityTypeConfiguration<WarehouseStock>
{
    public void Configure(EntityTypeBuilder<WarehouseStock> builder)
    {
        builder.ToTable("inventory_warehouse_stocks");
        builder.HasKey(ws => ws.Id);
        builder.Property(ws => ws.Id).HasColumnName("id");
        builder.Property(ws => ws.WarehouseId).HasColumnName("warehouse_id");
        builder.Property(ws => ws.ProductId).HasColumnName("product_id");
        builder.Property(ws => ws.OnHand).HasColumnName("on_hand").HasColumnType("numeric(12,3)");
        builder.Property(ws => ws.Reserved).HasColumnName("reserved").HasColumnType("numeric(12,3)");
        builder.Property(ws => ws.UpdatedAt).HasColumnName("updated_at");
        builder.HasIndex(ws => new { ws.WarehouseId, ws.ProductId }).IsUnique();
        builder.Ignore(ws => ws.Available);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("sales_invoice_items");
        builder.ConfigureAuditableEntity();
        builder.Property(ii => ii.InvoiceId).HasColumnName("invoice_id");
        builder.Property(ii => ii.LineNumber).HasColumnName("line_number");
        builder.Property(ii => ii.ProductId).HasColumnName("product_id");
        builder.Property(ii => ii.Description).HasColumnName("description");
        builder.Property(ii => ii.Quantity).HasColumnName("quantity").HasColumnType("numeric(12,3)");
        builder.Property(ii => ii.UnitPrice).HasColumnName("unit_price").HasColumnType("numeric(18,2)");
        builder.Property(ii => ii.DiscountRate).HasColumnName("discount_rate").HasColumnType("numeric(5,2)");
        builder.Property(ii => ii.DiscountAmount).HasColumnName("discount_amount").HasColumnType("numeric(18,2)");
        builder.Property(ii => ii.TaxTotal).HasColumnName("tax_total").HasColumnType("numeric(18,2)");
        builder.Property(ii => ii.LineTotal).HasColumnName("line_total").HasColumnType("numeric(18,2)");
        builder.Ignore(ii => ii.Taxes);
    }
}

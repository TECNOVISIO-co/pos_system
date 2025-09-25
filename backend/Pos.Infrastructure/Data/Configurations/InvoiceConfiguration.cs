using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("sales_invoices");
        builder.ConfigureAuditableEntity();
        builder.Property(i => i.InvoiceNumber).HasColumnName("invoice_number").IsRequired();
        builder.Property(i => i.CustomerId).HasColumnName("customer_id");
        builder.Property(i => i.Status).HasColumnName("status").HasConversion<string>();
        builder.Property(i => i.IssuedAt).HasColumnName("issued_at");
        builder.Property(i => i.DueAt).HasColumnName("due_at");
        builder.Property(i => i.WarehouseId).HasColumnName("warehouse_id");
        builder.Property(i => i.Subtotal).HasColumnName("subtotal").HasColumnType("numeric(18,2)");
        builder.Property(i => i.DiscountTotal).HasColumnName("discount_total").HasColumnType("numeric(18,2)");
        builder.Property(i => i.TaxTotal).HasColumnName("tax_total").HasColumnType("numeric(18,2)");
        builder.Property(i => i.Total).HasColumnName("total").HasColumnType("numeric(18,2)");
        builder.Property(i => i.TotalPaid).HasColumnName("total_paid").HasColumnType("numeric(18,2)");
        builder.Property(i => i.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(i => i.Notes).HasColumnName("notes");
        builder.Ignore(i => i.Payments);
        builder.Ignore(i => i.BalanceDue);
        builder.HasMany<InvoiceItem>().WithOne().HasForeignKey(ii => ii.InvoiceId);
        builder.Navigation(i => i.Items).HasField("_items");
        builder.Navigation(i => i.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

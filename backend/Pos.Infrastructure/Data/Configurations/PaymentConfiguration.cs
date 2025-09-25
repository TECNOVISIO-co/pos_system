using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("sales_payments");
        builder.ConfigureAuditableEntity();
        builder.Property(p => p.InvoiceId).HasColumnName("invoice_id");
        builder.Property(p => p.CustomerId).HasColumnName("customer_id");
        builder.Property(p => p.Method).HasColumnName("payment_method").HasConversion<string>();
        builder.Property(p => p.Reference).HasColumnName("reference");
        builder.Property(p => p.PaidAt).HasColumnName("paid_at");
        builder.Property(p => p.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)");
        builder.Property(p => p.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(p => p.ExchangeRate).HasColumnName("exchange_rate").HasColumnType("numeric(18,6)");
        builder.Property(p => p.Notes).HasColumnName("notes");
    }
}

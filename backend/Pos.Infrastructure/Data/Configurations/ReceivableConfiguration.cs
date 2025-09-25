using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class ReceivableConfiguration : IEntityTypeConfiguration<Receivable>
{
    public void Configure(EntityTypeBuilder<Receivable> builder)
    {
        builder.ToTable("accounting_receivables");
        builder.ConfigureAuditableEntity();
        builder.Property(r => r.CustomerId).HasColumnName("customer_id");
        builder.Property(r => r.OriginType).HasColumnName("origin_type").IsRequired();
        builder.Property(r => r.OriginId).HasColumnName("origin_id");
        builder.Property(r => r.IssuedAt).HasColumnName("issued_at");
        builder.Property(r => r.DueAt).HasColumnName("due_at");
        builder.Property(r => r.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)");
        builder.Property(r => r.Balance).HasColumnName("balance").HasColumnType("numeric(18,2)");
        builder.Property(r => r.CurrencyCode).HasColumnName("currency_code").HasMaxLength(3);
        builder.Property(r => r.Status).HasColumnName("status");
        builder.Ignore(r => r.Payments);
    }
}

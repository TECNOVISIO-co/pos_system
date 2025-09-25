using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class ReceivablePaymentConfiguration : IEntityTypeConfiguration<ReceivablePayment>
{
    public void Configure(EntityTypeBuilder<ReceivablePayment> builder)
    {
        builder.ToTable("accounting_receivable_payments");
        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Id).HasColumnName("id");
        builder.Property(rp => rp.ReceivableId).HasColumnName("receivable_id");
        builder.Property(rp => rp.PaymentId).HasColumnName("payment_id");
        builder.Property(rp => rp.Amount).HasColumnName("amount").HasColumnType("numeric(18,2)");
        builder.Property(rp => rp.AppliedAt).HasColumnName("applied_at");
        builder.Property(rp => rp.CreatedAt).HasColumnName("created_at");
        builder.Property(rp => rp.CreatedBy).HasColumnName("created_by");
    }
}

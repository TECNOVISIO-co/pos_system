using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("crm_customers");
        builder.ConfigureAuditableEntity();
        builder.Property(c => c.Code).HasColumnName("code").IsRequired();
        builder.Property(c => c.DocumentType).HasColumnName("document_type").IsRequired();
        builder.Property(c => c.DocumentNumber).HasColumnName("document_number").IsRequired();
        builder.Property(c => c.Name).HasColumnName("name").IsRequired();
        builder.Property(c => c.TradeName).HasColumnName("trade_name");
        builder.Property(c => c.Email).HasColumnName("email");
        builder.Property(c => c.Phone).HasColumnName("phone");
        builder.Property(c => c.Mobile).HasColumnName("mobile");
        builder.Property(c => c.Address).HasColumnName("address");
        builder.Property(c => c.City).HasColumnName("city");
        builder.Property(c => c.State).HasColumnName("state");
        builder.Property(c => c.Country).HasColumnName("country");
        builder.Property(c => c.PostalCode).HasColumnName("postal_code");
        builder.Property(c => c.CreditLimit).HasColumnName("credit_limit").HasColumnType("numeric(18,2)");
        builder.Property(c => c.AvailableCredit).HasColumnName("available_credit").HasColumnType("numeric(18,2)");
        builder.Property(c => c.PriceListId).HasColumnName("price_list_id");
        builder.Property(c => c.TaxResponsibility).HasColumnName("tax_responsibility");
        builder.Property(c => c.BirthDate).HasColumnName("birthdate");
        builder.Property(c => c.LoyaltyPoints).HasColumnName("loyalty_points").HasColumnType("numeric(12,3)");
        builder.HasIndex(c => c.Code).IsUnique();
    }
}

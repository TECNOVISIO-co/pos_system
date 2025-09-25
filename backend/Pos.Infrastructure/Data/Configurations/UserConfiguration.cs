using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.Domain.Entities;

namespace Pos.Infrastructure.Data.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("core_users");
        builder.ConfigureAuditableEntity();
        builder.Property(u => u.Username).HasColumnName("username").IsRequired();
        builder.Property(u => u.FullName).HasColumnName("full_name").IsRequired();
        builder.Property(u => u.Email).HasColumnName("email");
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(u => u.IsActive).HasColumnName("is_active");
        builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");
    }
}

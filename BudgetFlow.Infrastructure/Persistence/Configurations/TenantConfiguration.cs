using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetFlow.Infrastructure.Persistence.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Subdomain)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(t => t.Subdomain)
                .IsUnique();

            builder.Property(t => t.IsActive)
                .HasDefaultValue(true);

            builder.Property(t => t.Plan)
                .HasConversion<string>()
                .HasDefaultValue(SubscriptionPlan.Free);
        }
    }
}
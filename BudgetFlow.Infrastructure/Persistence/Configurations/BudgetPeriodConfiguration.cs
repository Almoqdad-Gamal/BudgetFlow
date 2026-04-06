using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetFlow.Infrastructure.Persistence.Configurations
{
    public class BudgetPeriodConfiguration : IEntityTypeConfiguration<BudgetPeriod>
    {
        public void Configure(EntityTypeBuilder<BudgetPeriod> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.AllocatedBudget)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.SpentAmount)
                .HasColumnType("decimal(18,2)");

            builder.Ignore(b => b.RemainingBudget);
            builder.Ignore(b => b.SpentPercentage);
            builder.Ignore(b => b.IsAlertThresholdReached);

            builder.HasIndex(b => new {b.DepartmentId, b.Month, b.Year})
                .IsUnique();

            builder.HasOne(b => b.Tenant)
                .WithMany()
                .HasForeignKey(b => b.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Department)
                .WithMany(d => d.BudgetPeriods)
                .HasForeignKey(b => b.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Tests
{
    public class TestDbContext : DbContext, IApplicationDbContext
    {
        public TestDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<BudgetPeriod> BudgetPeriods => Set<BudgetPeriod>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BudgetPeriod>()
                .Ignore(b => b.RemainingBudget)
                .Ignore(b => b.SpentPercentage)
                .Ignore(b => b.IsAlertThresholdReached);
        }

    }
}
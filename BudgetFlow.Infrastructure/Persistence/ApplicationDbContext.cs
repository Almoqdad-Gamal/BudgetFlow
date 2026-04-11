using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<BudgetPeriod> BudgetPeriods => Set<BudgetPeriod>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
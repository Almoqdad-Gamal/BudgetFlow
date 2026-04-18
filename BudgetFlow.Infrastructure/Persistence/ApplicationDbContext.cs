using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Common;
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

        // Insted of writing UpdatedAt in every handler, I've written it here once, and it's applied to all the entities
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if(entry.State == EntityState.Modified)
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
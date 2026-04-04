using BudgetFlow.Domain.Common;

namespace BudgetFlow.Domain.Entities
{
    public class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public int MaxDepartments { get; set; }
        public int MaxUsers { get; set; }

        // Navigation properties
        public ICollection<Department> Departments { get; set; } = new List<Department>();
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
    }
}
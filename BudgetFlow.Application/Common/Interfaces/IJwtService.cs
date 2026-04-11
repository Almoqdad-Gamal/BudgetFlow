using BudgetFlow.Domain.Entities;

namespace BudgetFlow.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(AppUser user);
        string GenerateRefreshToken();
    }
}
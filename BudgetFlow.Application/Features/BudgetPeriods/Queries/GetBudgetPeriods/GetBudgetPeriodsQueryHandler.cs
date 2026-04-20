using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Features.BudgetPeriods.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.BudgetPeriods.Queries.GetBudgetPeriods
{
    public class GetBudgetPeriodsQueryHandler : IRequestHandler<GetBudgetPeriodsQuery, List<BudgetPeriodDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetBudgetPeriodsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<BudgetPeriodDto>> Handle(GetBudgetPeriodsQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            var query = _context.BudgetPeriods
                .Include(b => b.Department)
                .Where(b => b.TenantId == tenantId)
                .AsQueryable();

            if(request.DepartmentId.HasValue)
                query = query.Where(b => b.DepartmentId == request.DepartmentId.Value);

            if(request.Month.HasValue)
                query = query.Where(b => b.Month == request.Month.Value);

            if(request.Year.HasValue)
                query = query.Where(b => b.Year == request.Year.Value);

            return await query
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .Select(b => new BudgetPeriodDto(
                    b.Id,
                    b.DepartmentId,
                    b.Department.Name,
                    b.Month,
                    b.Year,
                    b.AllocatedBudget,
                    b.SpentAmount,
                    // computed properties
                    b.AllocatedBudget - b.SpentAmount,
                    b.AllocatedBudget == 0 ? 0 : (b.SpentAmount / b.AllocatedBudget) * 100,
                    b.AllocatedBudget == 0 ? false : (b.SpentAmount / b.AllocatedBudget) * 100 >= 80
                )).ToListAsync(cancellationToken);
        }
    }
}
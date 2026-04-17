using BudgetFlow.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Departments.Queries.GetDepartments
{
    public class GetDepartmentsQueryHandler : IRequestHandler<GetDepartmentsQuery, List<DepartmentDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public GetDepartmentsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            return await _context.Departments
                .Where(d => d.TenantId == tenantId)
                .Select(d => new DepartmentDto(
                    d.Id,
                    d.Name,
                    d.BudgetLimit,
                    d.Currency
                )).ToListAsync(cancellationToken);
        }
    }
}
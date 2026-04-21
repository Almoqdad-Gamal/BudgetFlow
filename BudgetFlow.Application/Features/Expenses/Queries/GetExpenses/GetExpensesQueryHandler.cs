using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Features.Expenses.DTOs;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Expenses.Queries.GetExpenses
{
    public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, PagedResult<ExpenseDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetExpensesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ExpenseDto>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var role = _currentUserService.Role;
            var userId = _currentUserService.UserId;

            var query = _context.Expenses
                .Include(e => e.CreatedByUser)
                .Include(e => e.Department)
                .Where(e => e.TenantId == tenantId)
                .AsQueryable();

            // The Employee sees his his own expenses
            if (role == "Employee")
                query = query.Where(e => e.CreatedByUserId == userId);

            // Filter by department if it submitted in the request
            if (request.DepartmentId.HasValue)
                query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);

            // Filter by status if it submitted in the request
            if (!string.IsNullOrEmpty(request.Status) &&
                Enum.TryParse<ExpenseStatus>(request.Status, out var status))
                query = query.Where(e => e.Status == status);

            // Total count of records before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new ExpenseDto(
                    e.Id,
                    e.Title,
                    e.Amount,
                    e.Currency,
                    e.Status.ToString(),
                    $"{e.CreatedByUser.FirstName} {e.CreatedByUser.LastName}",
                    e.DepartmentId,
                    e.Department.Name,
                    e.CreatedAt
                )).ToListAsync(cancellationToken);

            return new PagedResult<ExpenseDto>(
                items,
                totalCount,
                request.Page,
                request.PageSize,
                (int)Math.Ceiling(totalCount / (double)request.PageSize)
            );
        }
    }
}
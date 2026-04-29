using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Features.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public GetUsersQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            return await _context.Users
                .Where(u => u.TenantId == tenantId)
                .Select(u => new UserDto
                (
                    u.Id,
                    $"{u.FirstName} {u.LastName}",
                    u.Email,
                    u.Role.ToString(),
                    u.IsActive
                )).ToListAsync(cancellationToken);
        }
    }
}
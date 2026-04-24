using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Features.AuditLogs.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.AuditLogs.Queries.GetAuditLogs
{
    public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedResult<AuditLogDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetAuditLogsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<AuditLogDto>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            var query = _context.AuditLogs
                .Where(a => a.TenantId == tenantId)
                .OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.page - 1) * request.pageSize)
                .Take(request.pageSize)
                .Select(a => new AuditLogDto(
                    a.Id,
                    a.Action,
                    a.EntityName,
                    a.OldValues,
                    a.NewValues,
                    a.IpAddress,
                    a.CreatedAt
                )).ToListAsync(cancellationToken);

            return new PagedResult<AuditLogDto>
            (
                items,
                totalCount,
                request.page,
                request.pageSize,
                (int)Math.Ceiling(totalCount / (double) request.pageSize)
            );
        }
    }
}
using BudgetFlow.Application.Common.Models;
using BudgetFlow.Application.Features.AuditLogs.DTOs;
using MediatR;

namespace BudgetFlow.Application.Features.AuditLogs.Queries.GetAuditLogs
{
    public record GetAuditLogsQuery 
    (
        int page = 1,
        int pageSize = 50
    ) : IRequest<PagedResult<AuditLogDto>>;
    
}
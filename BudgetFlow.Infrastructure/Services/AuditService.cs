using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IApplicationDbContext context, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(string action, string entityName, string? oldValues = null, string? newValues = null, CancellationToken cancellationToken = default)
        {
            // Get the IP adress from request
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

            var auditlog = new AuditLog
            {
                TenantId = _currentUserService.TenantId,
                UserId = _currentUserService.UserId,
                Action = action,
                EntityName = entityName,
                OldValues = oldValues,
                NewValues = newValues,
                IpAddress = ipAddress
            };

            _context.AuditLogs.Add(auditlog);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
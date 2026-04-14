using BudgetFlow.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.API.Middleware
{
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync (HttpContext context, ApplicationDbContext dbContext)
        {
            var host = context.Request.Host.Host;

            // Extract subdomain from host
            var subdomain = ExtractSubdomain(host);

            if(!string.IsNullOrEmpty(subdomain))
            {
                var tenant = await dbContext.Tenants
                    .Where(t => t.Subdomain == subdomain && t.IsActive)
                    .FirstOrDefaultAsync();

                if(tenant != null)
                {
                    context.Items["TenantId"] = tenant.Id;
                    context.Items["TenantName"] = tenant.Name;
                }
            }

            await _next(context);
        }

        private static string? ExtractSubdomain(string host)
        {
            if(host == "localhost" || host.StartsWith("127.") || host.StartsWith("192."))
                return null;

            var parts = host.Split('.');

            if(parts.Length >= 3)
                return parts[0];

            return null;
        }
    }
}
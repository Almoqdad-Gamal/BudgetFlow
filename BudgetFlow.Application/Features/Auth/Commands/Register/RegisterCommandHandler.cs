using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public RegisterCommandHandler(IApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var subdomainExists = await _context.Tenants
                .Where(t => t.Subdomain == request.Subdomain)
                .FirstOrDefaultAsync(cancellationToken) !=null;
            if(subdomainExists)
                throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                    "Subdomain", "This subdomain is already taken."
                )]);

            var tenant = new Tenant
            {
                Name = request.TenantName,
                Subdomain = request.Subdomain.ToLower(),
                IsActive = true,
                MaxDepartments = 2,
                MaxUsers = 5
            };

            var user = new AppUser
            {
                TenantId = tenant.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = Domain.Enums.UserRole.TenantAdmin,
                IsActive = true
            };

            _context.Tenants.Add(tenant);
            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            // Clear the change tracker to ensure subsequent reads hit the database
            if (_context is DbContext dbContext)
                dbContext.ChangeTracker.Clear();

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new RegisterResult(tenant.Id, user.Id, token, refreshToken);
        }
    }
}
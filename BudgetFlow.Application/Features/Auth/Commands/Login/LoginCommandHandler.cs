using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _context.Tenants
                .Where(t => t.Subdomain == request.Subdomain && t.IsActive)
                .FirstOrDefaultAsync(cancellationToken);

            if (tenant is null)
                throw new NotFoundException("Tenant", request.Subdomain);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower() && u.TenantId == tenant.Id && u.IsActive, cancellationToken);

            if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new ValidationException([
                    new FluentValidation.Results.ValidationFailure("Credentials", "Invalid email or password.")
                ]);

            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new LoginResult
            (
                user.Id,
                $"{user.FirstName} {user.LastName}",
                user.Role.ToString(),
                token,
                refreshToken
            );
        }
    }
}
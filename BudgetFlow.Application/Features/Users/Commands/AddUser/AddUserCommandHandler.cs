using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Users.Commands.AddUser
{
    public class AddUserCommandHandler: IRequestHandler<AddUserCommand, AddUserResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AddUserCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<AddUserResponse> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;

            // Get tenant to check MaxUsers
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

            if (tenant is null)
                throw new NotFoundException("Tenant", tenantId);

            // Check MaxUsers
            var currentUserCount = await _context.Users
                .CountAsync(u => u.TenantId == tenantId, cancellationToken);

            if (currentUserCount >= tenant.MaxUsers)
                throw new ForbiddenException(
                    $"You have reached the maximum number of users ({tenant.MaxUsers}) for your plan. "+
                    "Please upgrade to Pro to add more users.");

            // Check that email is not exist in the same Tenant
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email.ToLower() && u.TenantId == tenantId, cancellationToken);
            
            if (emailExists)
                throw new ValidationException([
                    new FluentValidation.Results.ValidationFailure(
                        "Email", "A user with this email already exists in your organization.")
                ]);

            // Parse the role from string
            if (!Enum.TryParse<UserRole>(request.Role, out var role))
                throw new ValidationException([
                    new FluentValidation.Results.ValidationFailure(
                        "Role", "Invalid role specified.")
                ]);

            var user = new AppUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = role,
                IsActive = true,
                TenantId = tenantId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddUserResponse
            (
                user.Id,
                $"{user.FirstName} {user.LastName}",
                user.Email,
                user.Role.ToString()
            );
        }
    }
}
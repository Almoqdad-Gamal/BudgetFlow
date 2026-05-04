using BudgetFlow.Application.Common.Exceptions;
using BudgetFlow.Application.Common.Interfaces;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BudgetFlow.Application.Features.Users.Commands.DeactivateUser
{
    public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, DeactivateUserResponse>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public DeactivateUserCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<DeactivateUserResponse> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
        {
            var tenantId = _currentUserService.TenantId;
            var currentUserId = _currentUserService.UserId;

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId && u.TenantId == tenantId, cancellationToken);

            if (user is null)
                throw new NotFoundException("User", request.UserId);

            // TenantAdmin cannot deactivate himself
            if (user.Id == currentUserId)
                throw new ForbiddenException("You cannot deactivate your own account.");

            // Cannot deactrivate another tenantAdmin
            if (user.Role == UserRole.TenantAdmin)
                throw new ForbiddenException("Cannot deactivate a TenantAdmin account.");

            // if it's already deactivate
            if (!user.IsActive)
                throw new ValidationException([
                    new FluentValidation.Results.ValidationFailure(
                        "UserId", "This user is already deactivated.")
                ]);

            user.IsActive = false;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);

            return new DeactivateUserResponse
            (
                user.Id,
                $"{user.FirstName} {user.LastName}",
                user.IsActive
            );
        }
    }
}
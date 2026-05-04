using MediatR;

namespace BudgetFlow.Application.Features.Users.Commands.DeactivateUser
{
    public record DeactivateUserCommand(Guid UserId) : IRequest<DeactivateUserResponse>;
    
}
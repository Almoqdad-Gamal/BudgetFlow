using MediatR;

namespace BudgetFlow.Application.Features.Users.Commands.AddUser
{
    public record AddUserCommand
    (
        string FirstName,
        string LastName,
        string Email,
        string Password,
        string Role
    ) : IRequest<AddUserResponse>;
}
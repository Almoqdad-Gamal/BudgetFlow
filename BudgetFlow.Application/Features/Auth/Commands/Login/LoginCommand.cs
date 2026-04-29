using MediatR;

namespace BudgetFlow.Application.Features.Auth.Commands.Login
{
    public record LoginCommand
    (
        string Email,
        string Password,
        string Subdomain
    ) : IRequest<LoginResponse>;
}
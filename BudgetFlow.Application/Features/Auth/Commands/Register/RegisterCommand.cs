using MediatR;

namespace BudgetFlow.Application.Features.Auth.Commands.Register
{
    public record RegisterCommand
    (
        string TenantName,
        string Subdomain,
        string FirstName,
        string LastName,
        string Email,
        string Password
    ) : IRequest<RegisterResponse>;
}
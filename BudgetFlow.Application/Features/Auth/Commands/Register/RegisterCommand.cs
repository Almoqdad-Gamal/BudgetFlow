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
    ) : IRequest<RegisterResult>;

    public record RegisterResult
    (
        Guid TenantId,
        Guid UserId,
        string Token,
        string RefreshToken
    );
}
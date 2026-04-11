using MediatR;

namespace BudgetFlow.Application.Features.Auth.Commands.Login
{
    public record LoginCommand
    (
        string Email,
        string Password,
        string Subdomain
    ) : IRequest<LoginResult>;

    public record LoginResult
    (
        Guid UserId,
        string FullName,
        string Role,
        string Token,
        string RefreshToken
    );
}
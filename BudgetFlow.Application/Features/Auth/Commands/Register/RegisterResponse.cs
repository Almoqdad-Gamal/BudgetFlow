namespace BudgetFlow.Application.Features.Auth.Commands.Register
{
    public record RegisterResponse
    (
        Guid TenantId,
        Guid UserId,
        string Token,
        string RefreshToken
    );
}
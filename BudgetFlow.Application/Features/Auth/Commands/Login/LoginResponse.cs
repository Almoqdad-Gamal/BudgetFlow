namespace BudgetFlow.Application.Features.Auth.Commands.Login
{
    public record LoginResponse
    (
        Guid UserId,
        string FullName,
        string Role,
        string Token,
        string RefreshToken
    );
}
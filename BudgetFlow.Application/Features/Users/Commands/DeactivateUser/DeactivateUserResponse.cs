namespace BudgetFlow.Application.Features.Users.Commands.DeactivateUser
{
    public record DeactivateUserResponse
    (
        Guid UserId,
        string FullName,
        bool IsActive
    );
}
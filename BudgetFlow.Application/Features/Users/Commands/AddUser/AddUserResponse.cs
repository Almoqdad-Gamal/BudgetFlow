namespace BudgetFlow.Application.Features.Users.Commands.AddUser
{
    public record AddUserResponse
    (
        Guid UserId,
        string FullName,
        string Email,
        string Role
    );
}
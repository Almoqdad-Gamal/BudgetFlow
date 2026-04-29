using BudgetFlow.Application.Features.Users.DTOs;
using MediatR;

namespace BudgetFlow.Application.Features.Users.Queries.GetUsers
{
    public record GetUsersQuery : IRequest<List<UserDto>>; 
    
}
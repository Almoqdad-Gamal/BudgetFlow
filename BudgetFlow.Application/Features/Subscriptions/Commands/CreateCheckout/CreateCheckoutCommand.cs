using MediatR;

namespace BudgetFlow.Application.Features.Subscriptions.Commands.CreateCheckout
{
    public record CreateCheckoutCommand : IRequest<CreateCheckoutResponse>;
}
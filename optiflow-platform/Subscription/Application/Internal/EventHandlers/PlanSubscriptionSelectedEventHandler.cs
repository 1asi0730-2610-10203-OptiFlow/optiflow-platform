using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class PlanSubscriptionSelectedEventHandler(
    ISubscriptionQueryService subscriptionQueryService,
    IPaymentCommandService paymentCommandService)
    : IEventHandler<PlanSubscriptionSelectedEvent>
{
    public Task Handle(PlanSubscriptionSelectedEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(PlanSubscriptionSelectedEvent domainEvent, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueryService.Handle(
            new GetSubscriptionByIdQuery(new SubscriptionId(domainEvent.SubscriptionId)), cancellationToken);

        if (subscription is null)
        {
            Console.WriteLine("Subscription not found for payment processing - SubscriptionId: " + domainEvent.SubscriptionId);
            return;
        }

        await paymentCommandService.Handle(
            new ProcessSubscriptionPaymentCommand(
                new SubscriptionId(domainEvent.SubscriptionId),
                subscription.Amount,
                subscription.PaymentMethod),
            cancellationToken);
    }
}

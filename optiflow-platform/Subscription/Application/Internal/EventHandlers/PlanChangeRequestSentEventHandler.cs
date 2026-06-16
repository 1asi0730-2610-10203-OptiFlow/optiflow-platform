using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class PlanChangeRequestSentEventHandler(
    ISubscriptionQueryService subscriptionQueryService,
    IPaymentCommandService paymentCommandService)
    : IEventHandler<PlanChangeRequestSentEvent>
{
    public Task Handle(PlanChangeRequestSentEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(PlanChangeRequestSentEvent domainEvent, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueryService.Handle(
            new GetSubscriptionByIdQuery(new SubscriptionId(domainEvent.SubscriptionId)), cancellationToken);

        if (subscription is null)
        {
            Console.WriteLine("Subscription not found for plan change payment - SubscriptionId: " + domainEvent.SubscriptionId);
            return;
        }

        // (Plan changed?) Process Subscription Payment for the new plan
        await paymentCommandService.Handle(
            new ProcessSubscriptionPaymentCommand(
                new SubscriptionId(domainEvent.SubscriptionId),
                subscription.Amount,
                subscription.PaymentMethod),
            cancellationToken);
    }
}

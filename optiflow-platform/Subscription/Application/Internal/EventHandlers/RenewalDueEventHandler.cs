using optiflow_platform.Shared.Application.Internal.EventHandlers;
using optiflow_platform.Shared.Application.Patterns;
using optiflow_platform.Subscription.Application.Errors;
using optiflow_platform.Subscription.Application.Services;
using optiflow_platform.Subscription.Domain.Model.Aggregates;
using optiflow_platform.Subscription.Domain.Model.Commands;
using optiflow_platform.Subscription.Domain.Model.Events;
using optiflow_platform.Subscription.Domain.Model.Queries;
using optiflow_platform.Subscription.Domain.Model.ValueObjects;

namespace optiflow_platform.Subscription.Application.Internal.EventHandlers;

public class RenewalDueEventHandler(
    ISubscriptionQueryService subscriptionQueryService,
    IPaymentCommandService paymentCommandService,
    IBillingCommandService billingCommandService,
    ISubscriptionCommandService subscriptionCommandService)
    : IEventHandler<RenewalDueEvent>
{
    public Task Handle(RenewalDueEvent domainEvent, CancellationToken cancellationToken)
    {
        return On(domainEvent, cancellationToken);
    }

    private async Task On(RenewalDueEvent domainEvent, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueryService.Handle(
            new GetSubscriptionByIdQuery(new SubscriptionId(domainEvent.SubscriptionId)), cancellationToken);

        if (subscription is null)
        {
            Console.WriteLine("Subscription not found for renewal - SubscriptionId: " + domainEvent.SubscriptionId);
            return;
        }

        var paymentResult = await paymentCommandService.Handle(
            new ProcessSubscriptionPaymentCommand(
                new SubscriptionId(domainEvent.SubscriptionId),
                subscription.Amount,
                subscription.PaymentMethod),
            cancellationToken);

        // (Payment authorization successfully?) Renew Subscription
        if (paymentResult is Result<Payment, ProcessSubscriptionPaymentError>.Success)
        {
            await billingCommandService.Handle(
                new RequestAutoRenewCommand(new SubscriptionId(domainEvent.SubscriptionId)),
                cancellationToken);
            return;
        }

        // (Payment fails on renewal?) Expire Subscription
        Console.WriteLine("Renewal payment failed - expiring subscription: " + domainEvent.SubscriptionId);
        await subscriptionCommandService.Handle(
            new ExpireSubscriptionCommand(new SubscriptionId(domainEvent.SubscriptionId)),
            cancellationToken);
    }
}

namespace optiflow_platform.Subscription.Application.Errors;

public enum RenewSubscriptionError
{
    SubscriptionNotFound,
    BillingNotFound,
    PaymentFailed,
    UnexpectedError
}

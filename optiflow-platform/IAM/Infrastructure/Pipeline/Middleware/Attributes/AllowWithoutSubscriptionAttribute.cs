using System;

namespace optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marks an endpoint as reachable by an authenticated account holder even when their account has
///     no active subscription — e.g. the plan catalog, checkout, onboarding, profile, and the whole
///     client-facing patient portal. Everything else account-scoped requires an active subscription.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowWithoutSubscriptionAttribute : Attribute
{
}

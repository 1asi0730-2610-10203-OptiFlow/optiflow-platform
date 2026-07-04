using System;

namespace optiflow_platform.IAM.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marks an endpoint as reachable by an authenticated user who hasn't finished onboarding
///     (i.e. whose <c>User.AccountId</c> is still null) — e.g. the "create my account" endpoint itself.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowWithoutAccountAttribute : Attribute
{
}

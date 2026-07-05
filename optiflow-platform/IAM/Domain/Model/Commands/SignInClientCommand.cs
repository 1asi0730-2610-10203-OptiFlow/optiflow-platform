using optiflow_platform.IAM.Domain.Model.ValueObjects;

namespace optiflow_platform.IAM.Domain.Model.Commands;

/// <summary>
///     Passwordless sign-in for a client (patient) user. Clients are provisioned by their optic and
///     the app only tracks their order progress — no payment data — so a username (email) is enough.
/// </summary>
public record SignInClientCommand(EmailAddress Email);

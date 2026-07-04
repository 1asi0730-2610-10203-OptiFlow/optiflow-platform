namespace optiflow_platform.Shared.Application.Services;

/// <summary>
///     Exposes the identity of the caller resolved from the current request's JWT claims.
/// </summary>
public interface ICurrentUserContext
{
    Guid? UserId { get; }
    Guid? AccountId { get; }
}

using Cortex.Mediator.Notifications;

namespace optiflow_platform.Shared.Domain.Model.Events;

/// <summary>
///     Marker interface for all domain events in the system.
/// </summary>
/// <remarks>
///     Extends <see cref="INotification"/> so Cortex.Mediator can discover
///     and dispatch events to their handlers automatically.
/// </remarks>
public interface IEvent : INotification
{
}

using Cortex.Mediator.Notifications;
using optiflow_platform.Shared.Domain.Model.Events;

namespace optiflow_platform.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
}

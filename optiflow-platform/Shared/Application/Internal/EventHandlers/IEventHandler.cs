namespace optiflow_platform.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent>
{
    Task Handle(TEvent domainEvent, CancellationToken cancellationToken);
}

using MotoHub.Application.Events;

namespace MotoHub.Application.Interfaces.Messaging;

public interface IMotorcycleEventPublisher
{
    Task PublishMotorcycleRegisteredAsync(MotorcycleRegisteredEvent @event, CancellationToken cancellationToken);
}
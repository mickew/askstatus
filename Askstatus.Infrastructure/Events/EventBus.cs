using Askstatus.Application.Events;

namespace Askstatus.Infrastructure.Events;
internal sealed class EventBus : IEventBus
{
    private readonly InMemoryMessageQueue _messageQueue;

    public EventBus(InMemoryMessageQueue messageQueue)
    {
        _messageQueue = messageQueue;
    }

    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : class, IIntegrationEvent
    {
        await _messageQueue.Writer.WriteAsync(integrationEvent, cancellationToken);
    }
}

using Askstatus.Application.Sensors;
using Askstatus.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Infrastructure.Events;
internal sealed class SensorValueChangedEventHandler : INotificationHandler<SensorValueChangedIntegrationEvent>
{
    private readonly ILogger<SensorValueChangedEventHandler> _logger;
    private readonly IHubContext<StatusHub, IStatusClient> _hubContext;

    public SensorValueChangedEventHandler(ILogger<SensorValueChangedEventHandler> logger, IHubContext<StatusHub, IStatusClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public Task Handle(SensorValueChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sensor value changed: {SensorId}, {NewValue}, {TimeStamp}", notification.SensorId, notification.NewValue, notification.TimeStamp);
        return _hubContext.Clients.All.SensorValueChanged(notification.SensorId, notification.NewValue, notification.TimeStamp);
    }
}

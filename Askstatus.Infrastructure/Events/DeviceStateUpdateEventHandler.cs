using Askstatus.Application.PowerDevice;
using Askstatus.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Infrastructure.Events;
internal sealed class DeviceStateUpdateEventHandler : INotificationHandler<DeviceStateChangedIntegrationEvent>
{
    private readonly ILogger<DeviceStateChangedIntegrationEvent> _logger;
    private readonly IHubContext<StatusHub, IStatusClient> _hubContext;

    public DeviceStateUpdateEventHandler(ILogger<DeviceStateChangedIntegrationEvent> logger, IHubContext<StatusHub, IStatusClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Handle(DeviceStateChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Device state changed for device with id {Id} to {State}", notification.Id, notification.State);
        await _hubContext.Clients.All.UpdateDeviceStatus(notification.DeviceId, notification.State);
    }
}

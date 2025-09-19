using System.Runtime.CompilerServices;
using Askstatus.Application.PowerDevice;
using Askstatus.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

[assembly: InternalsVisibleTo("Askstatus.Infrastructure.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Askstatus.Infrastructure.Events;
internal sealed class DeviceStateUpdateEventHandler : INotificationHandler<DeviceStateChangedIntegrationEvent>
{
    private readonly ILogger<DeviceStateUpdateEventHandler> _logger;
    private readonly IHubContext<StatusHub, IStatusClient> _hubContext;

    public DeviceStateUpdateEventHandler(ILogger<DeviceStateUpdateEventHandler> logger, IHubContext<StatusHub, IStatusClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Handle(DeviceStateChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Device state changed for device with id {Id} to {State}", notification.Id, notification.State);
        await _hubContext.Clients.All.UpdateDeviceStatus(notification.DeviceId, notification.State);
    }
}

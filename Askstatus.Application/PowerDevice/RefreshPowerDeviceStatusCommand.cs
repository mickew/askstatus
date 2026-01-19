using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record SRefreshPowerDeviceStatusCommand : IRequest<Result>;

public sealed class RefreshPowerDeviceStatusCommandHandler : IRequestHandler<SRefreshPowerDeviceStatusCommand, Result>
{
    private readonly ILogger<RefreshPowerDeviceStatusCommandHandler> _logger;
    private readonly IMqttClientService _mqttClientService;

    public RefreshPowerDeviceStatusCommandHandler(ILogger<RefreshPowerDeviceStatusCommandHandler> logger, IMqttClientService mqttClientService)
    {
        _logger = logger;
        _mqttClientService = mqttClientService;
    }

    public async Task<Result> Handle(SRefreshPowerDeviceStatusCommand request, CancellationToken cancellationToken)
    {
        // Implementation logic to refresh power device status goes here.
        await _mqttClientService.RefreshStatusAsync();
        return Result.Ok();
    }
}

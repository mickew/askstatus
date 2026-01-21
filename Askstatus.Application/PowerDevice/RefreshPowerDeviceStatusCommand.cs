using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record RefreshPowerDeviceStatusCommand : IRequest<Result>;

public sealed class RefreshPowerDeviceStatusCommandHandler : IRequestHandler<RefreshPowerDeviceStatusCommand, Result>
{
    private readonly ILogger<RefreshPowerDeviceStatusCommandHandler> _logger;
    private readonly IMqttClientService _mqttClientService;

    public RefreshPowerDeviceStatusCommandHandler(ILogger<RefreshPowerDeviceStatusCommandHandler> logger, IMqttClientService mqttClientService)
    {
        _logger = logger;
        _mqttClientService = mqttClientService;
    }

    public async Task<Result> Handle(RefreshPowerDeviceStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _mqttClientService.RefreshStatusAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh devices status");
        }
        return Result.Fail(new ServerError("Failed to refresh devices status"));
    }
}

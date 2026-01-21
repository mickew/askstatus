using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record RefreshPowerDevicesCommand : IRequest<Result>;

public sealed class RefreshPowerDevicesCommandHandler : IRequestHandler<RefreshPowerDevicesCommand, Result>
{
    private readonly ILogger<RefreshPowerDevicesCommandHandler> _logger;
    private readonly IMqttClientService _mqttClientService;

    public RefreshPowerDevicesCommandHandler(ILogger<RefreshPowerDevicesCommandHandler> logger, IMqttClientService mqttClientService)
    {
        _logger = logger;
        _mqttClientService = mqttClientService;
    }
    public async Task<Result> Handle(RefreshPowerDevicesCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _mqttClientService.RefreshShellieDevicesAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to refresh devices");
        }
        return Result.Fail(new ServerError("Failed to refresh devices"));
    }
}

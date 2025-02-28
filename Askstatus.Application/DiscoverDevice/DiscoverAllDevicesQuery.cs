using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Askstatus.Common.PowerDevice;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.DiscoverDevice;

public sealed record DiscoverAllDevicesQuery() : IRequest<Result<IEnumerable<DicoverInfo>>>;

public sealed class DiscoverAllDevicesQueryHandler : IRequestHandler<DiscoverAllDevicesQuery, Result<IEnumerable<DicoverInfo>>>
{
    private readonly ILogger<DiscoverAllDevicesQueryHandler> _logger;
    private readonly IMqttClientService _mqttClientService;
    public DiscoverAllDevicesQueryHandler(ILogger<DiscoverAllDevicesQueryHandler> logger, IMqttClientService mqttClientService)
    {
        _logger = logger;
        _mqttClientService = mqttClientService;
    }
    public async Task<Result<IEnumerable<DicoverInfo>>> Handle(DiscoverAllDevicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var devices = await _mqttClientService.GetShellieDevicesAsync();
            return Result.Ok(devices.Select(x => new DicoverInfo(x.Ip, (PowerDeviceTypes)x.Gen, x.Name, x.Id, x.Mac, x.Model, 0)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get devices");
        }
        return Result.Fail(new ServerError("Failed to get devices"));
    }
}

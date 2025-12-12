using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Sensor;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public sealed record DiscoverAllSensorsQuery() : IRequest<Result<IEnumerable<SensorInfo>>>;

public sealed class DiscoverAllSensorsQueryHandler : IRequestHandler<DiscoverAllSensorsQuery, Result<IEnumerable<SensorInfo>>>
{
    private readonly ILogger<DiscoverAllSensorsQueryHandler> _logger;
    private readonly IMqttClientService _mqttClientService;
    public DiscoverAllSensorsQueryHandler(IMqttClientService mqttClientService, ILogger<DiscoverAllSensorsQueryHandler> logger)
    {
        _mqttClientService = mqttClientService;
        _logger = logger;
    }
    public async Task<Result<IEnumerable<SensorInfo>>> Handle(DiscoverAllSensorsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var sensors = await _mqttClientService.GetSensorsAsync();
            return Result.Ok(sensors.Select(x => new SensorInfo(x.Id, x.Name, x.Model, x.Values.Select(v => new SensorValue(v.Name, v.Value, v.LastUpdate)).ToList())));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sensors");
        }
        return Result.Fail(new ServerError("Failed to get sensors"));
    }
}

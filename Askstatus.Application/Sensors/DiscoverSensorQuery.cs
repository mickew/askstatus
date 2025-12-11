using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Sensor;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public sealed record DiscoverSensorQuery() : IRequest<Result<IEnumerable<SensorInfo>>>;

public sealed class DiscoverSensorQueryHandler : IRequestHandler<DiscoverSensorQuery, Result<IEnumerable<SensorInfo>>>
{
    private readonly ILogger<DiscoverSensorQueryHandler> _logger;
    private readonly IMqttClientService _mqttClientService;
    private readonly IUnitOfWork _unitOfWork;

    public DiscoverSensorQueryHandler(ILogger<DiscoverSensorQueryHandler> logger, IMqttClientService mqttClientService, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mqttClientService = mqttClientService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<SensorInfo>>> Handle(DiscoverSensorQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var assignedSensors = await _unitOfWork.SensorRepository.ListAllAsync();
            var sensors = await _mqttClientService.GetSensorsAsync();

            var resultList = new List<SensorInfo>();
            foreach (var sensor in sensors)
            {
                SensorInfo? info = null;
                if (assignedSensors.Any(s => s.SensorName == sensor.Id))
                {
                    info = new SensorInfo(sensor.Id, sensor.Name, sensor.Model, new List<SensorValue>());

                    foreach (var value in sensor.Values)
                    {
                        if (assignedSensors.Any(s => s.SensorName == sensor.Id && s.ValueName == value.Name))
                        {
                            continue;
                        }
                        info.Values.Add(new SensorValue(value.Name, value.Value, value.LastUpdate));
                    }
                }
                else
                {
                    info = sensors.Where(s => s.Id == sensor.Id).Select(s => new SensorInfo(s.Id, s.Name, s.Model, s.Values.Select(v => new SensorValue(v.Name, v.Value, v.LastUpdate)).ToList())).FirstOrDefault();
                }
                if (info is not null)
                {
                    resultList.Add(info);
                }
            }
            return Result.Ok<IEnumerable<SensorInfo>>(resultList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sensors");
        }
        return Result.Fail(new ServerError("Failed to get sensors"));
    }
}

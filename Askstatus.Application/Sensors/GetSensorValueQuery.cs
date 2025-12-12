using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Sensor;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public sealed record GetSensorValueQuery(int Id) : IRequest<Result<SensorValue>>;

public sealed class GetSensorValueQueryHandler : IRequestHandler<GetSensorValueQuery, Result<SensorValue>>
{
    private readonly IMqttClientService _mqttClientService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetSensorValueQueryHandler> _logger;

    public GetSensorValueQueryHandler(IUnitOfWork unitOfWork, IMqttClientService mqttClientService, ILogger<GetSensorValueQueryHandler> logger)
    {
        _mqttClientService = mqttClientService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    //TODO: Implement tests for Handle
    public async Task<Result<SensorValue>> Handle(GetSensorValueQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var sensor = await _unitOfWork.SensorRepository.GetByIdAsync(request.Id);
            if (sensor is null)
            {
                _logger.LogWarning("Sensor with id {SensorId} not found", request.Id);
                return Result.Fail<SensorValue>(new NotFoundError("Sensor not found"));
            }

            var sensors = await _mqttClientService.GetSensorsAsync();

            var sensorValue = sensors.Where(s => s.Id == sensor.SensorName).Select(s => s.Values.FirstOrDefault(v => v.Name == sensor.ValueName))!.FirstOrDefault();

            if (sensorValue is null)
            {
                _logger.LogDebug("Sensor value {SensorValue} for sensor {SensorName} not found", sensor.ValueName, sensor.SensorName);
                return Result.Fail<SensorValue>(new NotFoundError("Sensor value not found"));
            }
            var formatedValue = sensorValue.Value;
            if (!string.IsNullOrEmpty(sensor.FormatString))
            {
                formatedValue = string.Format(sensor.FormatString, sensorValue.Value);
            }
            return Result.Ok(new SensorValue(sensorValue.Name, formatedValue, sensorValue.LastUpdate));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting sensor value");
            return Result.Fail<SensorValue>(new ServerError("An unexpected error occurred"));
        }
    }
}

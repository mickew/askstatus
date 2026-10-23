using Askstatus.Application.Interfaces;
using Askstatus.Application.Sensors;
using Askstatus.Common.Sensor;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Askstatus.Infrastructure.Jobs;

[DisallowConcurrentExecution]
internal class PublishToMqttJob : IJob
{
    private readonly ILogger<PublishToMqttJob> _logger;
    private readonly ISender _sender;
    private readonly IMqttClientService _mqttClientService;

    public PublishToMqttJob(ILogger<PublishToMqttJob> logger, ISender sender, IMqttClientService mqttClientService)
    {
        _logger = logger;
        _sender = sender;
        _mqttClientService = mqttClientService;
    }

    public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("PublishToMqttJob executed at {time}", DateTimeOffset.Now);
        _ = PublishSelectedSensors();
        return ValueTask.CompletedTask;
    }

    private async Task PublishSelectedSensors()
    {
        try
        {
            var sensorsResult = await _sender.Send(new GetSensorsQuery());
            var sensors = sensorsResult.Value.Where((SensorDto s) => s.MQTTEnabled).Select(s => new SensorValue
            {
                Id = s.Id,
                Name = s.Name,
                SensorType = s.SensorType,
                Value = "--",
                LastUpdate = DateTime.MinValue
            });

            var sensorTasks = new List<Task>();
            foreach (var sensor in sensors)
            {
                sensorTasks.Add(Task.Run(async () =>
                {
                    await GetSensorValue(sensor);
                }));
            }

            await Task.WhenAll(sensorTasks);
            await _mqttClientService.PublishSelectedSensors("askstatus/sensors", System.Text.Json.JsonSerializer.Serialize(sensors));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing selected sensors to MQTT");
        }
    }

    private async Task GetSensorValue(SensorValue sensor)
    {
        try
        {
            var sensorValueResult = await _sender.Send(new GetSensorValueQuery(sensor.Id));
            if (sensorValueResult.IsSuccess)
            {
                var sensorValue = sensorValueResult.Value;
                _logger.LogDebug("Sensor {sensorName} value: {sensorValue}", sensor.Name, sensorValue);
                sensor.Value = sensorValue.Value;
                sensor.LastUpdate = sensorValue.LastUpdate;
            }
            else
            {
                _logger.LogWarning("Failed to get value for sensor {sensorName}: {error}", sensor.Name, sensorValueResult.Errors.FirstOrDefault()?.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value for sensor {sensorName}", sensor.Name);
        }
    }

    class SensorValue
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public SensorType SensorType { get; init; }
        public string? Value { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}

using Askstatus.Application.Events;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public record UpdateSensorValueCommand(string SensorName, string ValueName, string NewValue, DateTime TimeStamp) : IRequest<Result>;

// TODO: Add tests
public class UpdateSensorValueCommandHandler : IRequestHandler<UpdateSensorValueCommand, Result>
{
    private readonly ILogger<UpdateSensorValueCommandHandler> _logger;
    private readonly IEventBus _eventBus;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSensorValueCommandHandler(ILogger<UpdateSensorValueCommandHandler> logger, IEventBus eventBus, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _eventBus = eventBus;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateSensorValueCommand request, CancellationToken cancellationToken)
    {
        var sensor = await _unitOfWork.SensorRepository.GetBy(s => s.SensorName == request.SensorName && s.ValueName == request.ValueName);
        if (sensor is null)
        {
            _logger.LogDebug("Sensor with name {SensorName} and value name {ValueName} not found.", request.SensorName, request.ValueName);
            return Result.Ok();
            //return Result.Fail($"Sensor with name {request.SensorName} and value name {request.ValueName} not found.");
        }
        var formatedValue = request.NewValue;
        if (!string.IsNullOrEmpty(sensor.FormatString))
        {
            if (ParseSensor.TryParseValue(request.NewValue, sensor, out var result))
            {
                formatedValue = string.Format(sensor.FormatString, result);
                // maybe log the value in the future
            }
            else
            {
                _logger.LogWarning("Failed to parse sensor value {NewValue} for sensor {SensorName} and value name {ValueName}.", request.NewValue, request.SensorName, request.ValueName);
            }
        }
        await _eventBus.PublishAsync(new SensorValueChangedIntegrationEvent(Guid.NewGuid(), sensor.Id, formatedValue, request.TimeStamp), cancellationToken);
        return Result.Ok();
    }
}

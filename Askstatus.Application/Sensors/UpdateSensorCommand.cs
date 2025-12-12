using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Sensor;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public sealed record UpdateSensorCommand() : IRequest<Result>
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public SensorType SensorType { get; init; }
    public string? FormatString { get; init; }
    public string? SensorName { get; init; }
    public string? SensorModel { get; init; }
    public string? ValueName { get; init; }
}

public sealed class UpdateSensorCommandHandler : IRequestHandler<UpdateSensorCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateSensorCommandHandler> _logger;
    public UpdateSensorCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdateSensorCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result> Handle(UpdateSensorCommand request, CancellationToken cancellationToken)
    {
        var sensor = await _unitOfWork.SensorRepository.GetByIdAsync(request.Id);
        if (sensor is null)
        {
            _logger.LogWarning("Sensor with id {Id} not found", request.Id);
            return Result.Fail(new NotFoundError($"Sensor not found"));
        }
        sensor.Name = request.Name!;
        sensor.SensorType = request.SensorType;
        sensor.SensorName = request.SensorName!;
        sensor.FormatString = request.FormatString!;
        sensor.SensorModel = request.SensorModel!;
        sensor.ValueName = request.ValueName!;
        var result = await _unitOfWork.SensorRepository.UpdateAsync(sensor);
        if (!result)
        {
            _logger.LogError("Error updating Sensor with id {Id}", request.Id);
            return Result.Fail(new BadRequestError("Error updating Sensor"));
        }
        var ret = await _unitOfWork.SaveChangesAsync();
        if (ret == -1)
        {
            _logger.LogError("Error saving changes");
            return Result.Fail(new BadRequestError("Error saving changes"));
        }
        return Result.Ok();
    }
}

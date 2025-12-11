using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Sensor;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public sealed record CreateSensorCommand() : IRequest<Result<SensorDto>>
{
    public string Name { get; init; } = null!;
    public SensorType SensorType { get; init; }
    public string FormatString { get; init; } = null!;
    public string SensorName { get; init; } = null!;
    public string SensorModel { get; init; } = null!;
    public string ValueName { get; init; } = null!;
}

public sealed class CreateSensorCommandHandler : IRequestHandler<CreateSensorCommand, Result<SensorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateSensorCommandHandler> _logger;

    public CreateSensorCommandHandler(IUnitOfWork unitOfWork, ILogger<CreateSensorCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SensorDto>> Handle(CreateSensorCommand request, CancellationToken cancellationToken)
    {
        var sensor = new Askstatus.Domain.Entities.Sensor
        {
            Name = request.Name,
            SensorType = request.SensorType,
            FormatString = request.FormatString,
            SensorName = request.SensorName,
            SensorModel = request.SensorModel,
            ValueName = request.ValueName
        };

        var result = await _unitOfWork.SensorRepository.AddAsync(sensor);
        if (result is null)
        {
            _logger.LogError("Error creating Sensor");
            return Result.Fail<SensorDto>(new BadRequestError("Error creating Sensor"));
        }
        var ret = await _unitOfWork.SaveChangesAsync();
        if (ret == -1)
        {
            _logger.LogError("Error saving changes");
            return Result.Fail<SensorDto>(new BadRequestError("Error saving changes"));
        }

        return Result.Ok(new SensorDto(result.Id, result.Name, result.SensorType, result.FormatString, result.SensorName, result.SensorModel, result.ValueName));
    }
}

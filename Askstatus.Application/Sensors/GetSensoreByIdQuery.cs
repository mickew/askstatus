using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Sensor;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public sealed record GetSensoreByIdQuery(int Id) : IRequest<Result<SensorDto>>;

public sealed class GetSensoreByIdQueryHandler : IRequestHandler<GetSensoreByIdQuery, Result<SensorDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetSensoreByIdQueryHandler> _logger;

    public GetSensoreByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetSensoreByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<SensorDto>> Handle(GetSensoreByIdQuery request, CancellationToken cancellationToken)
    {
        var sensor = await _unitOfWork.SensorRepository.GetByIdAsync(request.Id);
        if (sensor == null)
        {
            _logger.LogWarning("Sensor with id {SensorId} not found", request.Id);
            return Result.Fail<SensorDto>(new NotFoundError("Sensor not found"));
        }

        var sensorDto = new SensorDto(sensor.Id, sensor.Name, sensor.SensorType, sensor.FormatString, sensor.SensorName, sensor.ValueName);

        return Result.Ok(sensorDto);
    }
}


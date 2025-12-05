using Askstatus.Application.Interfaces;
using Askstatus.Common.Sensor;
using FluentResults;
using MediatR;

namespace Askstatus.Application.Sensors;
public sealed record GetSensorsQuery() : IRequest<Result<IEnumerable<SensorDto>>>;

public sealed class GetSensorsQueryHandler : IRequestHandler<GetSensorsQuery, Result<IEnumerable<SensorDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSensorsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<SensorDto>>> Handle(GetSensorsQuery request, CancellationToken cancellationToken)
    {
        var sensors = await _unitOfWork.SensorRepository.ListAllAsync();
        return Result.Ok(sensors.Select(s => new SensorDto(s.Id, s.Name, s.SensorType, s.FormatString, s.SensorName, s.ValueName)));
    }
}

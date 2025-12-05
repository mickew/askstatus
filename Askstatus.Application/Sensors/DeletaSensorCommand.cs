using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.Sensors;
public sealed record DeleteSensorCommand(int Id) : IRequest<Result>;

public sealed class DeleteSensorCommandHandler : IRequestHandler<DeleteSensorCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSensorCommandHandler> _logger;
    public DeleteSensorCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSensorCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result> Handle(DeleteSensorCommand request, CancellationToken cancellationToken)
    {
        var sensor = await _unitOfWork.SensorRepository.GetByIdAsync(request.Id);
        if (sensor == null)
        {
            _logger.LogWarning("Sensor with id {Id} not found", request.Id);
            return Result.Fail(new NotFoundError($"Sensor not found"));
        }
        var result = await _unitOfWork.SensorRepository.DeleteAsync(sensor);
        if (!result)
        {
            _logger.LogError("Error deleting Sensor with id {Id}", request.Id);
            return Result.Fail(new BadRequestError("Error deleting Sensor"));
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

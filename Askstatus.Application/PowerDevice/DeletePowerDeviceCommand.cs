using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record DeletePowerDeviceCommand(int Id) : IRequest<Result>;

public sealed class DeletePowerDeviceCommandHandler : IRequestHandler<DeletePowerDeviceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeletePowerDeviceCommandHandler> _logger;

    public DeletePowerDeviceCommandHandler(IUnitOfWork unitOfWork, ILogger<DeletePowerDeviceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result> Handle(DeletePowerDeviceCommand request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetByIdAsync(request.Id);
        if (powerDevice == null)
        {
            _logger.LogWarning("PowerDevice with id {Id} not found", request.Id);
            return Result.Fail(new NotFoundError($"PowerDevice not found"));
        }
        var result = await _unitOfWork.PowerDeviceRepository.DeleteAsync(powerDevice);
        if (!result)
        {
            _logger.LogError("Error deleting PowerDevice with id {Id}", request.Id);
            return Result.Fail(new BadRequestError("Error deleting PowerDevice"));
        }

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}

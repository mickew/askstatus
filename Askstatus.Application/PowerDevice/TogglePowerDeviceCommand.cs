using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record TogglePowerDeviceCommand(int id) : IRequest<Result>;

public sealed class TogglePowerDeviceCommandHandler : IRequestHandler<TogglePowerDeviceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TogglePowerDeviceCommandHandler> _logger;
    private readonly IDeviceService _deviceService;

    public TogglePowerDeviceCommandHandler(IUnitOfWork unitOfWork, ILogger<TogglePowerDeviceCommandHandler> logger, IDeviceService deviceService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _deviceService = deviceService;
    }

    public async Task<Result> Handle(TogglePowerDeviceCommand request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetByIdAsync(request.id);
        if (powerDevice is null)
        {
            _logger.LogError("PowerDevice with Id: {id} not found", request.id);
            return Result.Fail(new NotFoundError("PowerDevice not found"));
        }
        else
        {
            var result = await _deviceService.Toggle(powerDevice.HostName, 0);
            return result;
        }
    }
}

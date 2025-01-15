using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;

public sealed record SwitchPowerDeviceCommand(int id, bool onOff) : IRequest<Result>;

public sealed class SwitchPowerDeviceCommandHandler : IRequestHandler<SwitchPowerDeviceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SwitchPowerDeviceCommandHandler> _logger;
    private readonly IDeviceService _deviceService;
    public SwitchPowerDeviceCommandHandler(IUnitOfWork unitOfWork, ILogger<SwitchPowerDeviceCommandHandler> logger, IDeviceService deviceService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _deviceService = deviceService;
    }
    public async Task<Result> Handle(SwitchPowerDeviceCommand request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetByIdAsync(request.id);
        if (powerDevice is null)
        {
            _logger.LogError("PowerDevice with Id: {id} not found", request.id);
            return Result.Fail(new NotFoundError("PowerDevice not found"));
        }
        else
        {
            var result = await _deviceService.Switch(powerDevice.HostName, 0, request.onOff);
            return result;
        }
    }
}

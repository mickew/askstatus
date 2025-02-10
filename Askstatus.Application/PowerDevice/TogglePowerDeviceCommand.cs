using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record TogglePowerDeviceCommand(int id, string User) : IRequest<Result>;

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
            try
            {
                await _unitOfWork.SystemLogRepository.AddAsync(new Askstatus.Domain.Entities.SystemLog
                {
                    EventTime = DateTime.UtcNow,
                    EventType = SystemLogEventType.SetDeviceState,
                    User = request.User,
                    Message = $"Toggle PowerDevice {powerDevice.Name} to On/Off"
                });
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving SystemLog");
            }
            var result = await _deviceService.Toggle(powerDevice.HostName, 0);
            return result;
        }
    }
}

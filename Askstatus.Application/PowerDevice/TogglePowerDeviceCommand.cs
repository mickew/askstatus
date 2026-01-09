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
    private readonly IMqttClientService _mqttClientService;

    public TogglePowerDeviceCommandHandler(IUnitOfWork unitOfWork, ILogger<TogglePowerDeviceCommandHandler> logger, IMqttClientService mqttClientService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mqttClientService = mqttClientService;
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
            var mqttResult = await _mqttClientService.ToggleDeviceAsync(powerDevice.DeviceId, powerDevice.Channel);
            return mqttResult ? Result.Ok() : Result.Fail(new ServerError("Failed to toggle device via MQTT"));
        }
    }
}

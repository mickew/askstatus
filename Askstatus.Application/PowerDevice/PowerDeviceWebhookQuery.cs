using Askstatus.Application.Errors;
using Askstatus.Application.Events;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;

public sealed record PowerDeviceWebhookQuery(string Mac, bool state) : IRequest<Result>;

public sealed class PowerDeviceWebhookQueryHandler : IRequestHandler<PowerDeviceWebhookQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PowerDeviceWebhookQueryHandler> _logger;
    private readonly IEventBus _eventBus;

    public PowerDeviceWebhookQueryHandler(IUnitOfWork unitOfWork, ILogger<PowerDeviceWebhookQueryHandler> logger, IEventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _eventBus = eventBus;
    }
    public async Task<Result> Handle(PowerDeviceWebhookQuery request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetBy(x => x.DeviceMac == request.Mac);
        if (powerDevice == null)
        {
            _logger.LogWarning("PowerDevice with mac {Mac} not found", request.Mac);
            return Result.Fail(new NotFoundError($"PowerDevice not found"));
        }
        else
        {
            try
            {
                await _unitOfWork.SystemLogRepository.AddAsync(new Askstatus.Domain.Entities.SystemLog
                {
                    EventTime = DateTime.UtcNow,
                    EventType = SystemLogEventType.SetDeviceState,
                    User = "System",
                    Message = $"PowerDevice {powerDevice.Name} switched to {request.state}"
                });
                await _unitOfWork.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving SystemLog");
            }
        }

        await _eventBus.PublishAsync(new DeviceStateChangedIntegrationEvent(Guid.NewGuid(), powerDevice.Id, request.state), cancellationToken);
        return Result.Ok();
    }
}

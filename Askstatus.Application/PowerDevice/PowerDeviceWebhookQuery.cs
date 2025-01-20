using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;

public sealed record PowerDeviceWebhookQuery(string Mac, bool state) : IRequest<Result>;

public sealed class PowerDeviceWebhookQueryHandler : IRequestHandler<PowerDeviceWebhookQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PowerDeviceWebhookQueryHandler> _logger;
    private readonly IPublisher _publisher;

    public PowerDeviceWebhookQueryHandler(IUnitOfWork unitOfWork, ILogger<PowerDeviceWebhookQueryHandler> logger, IPublisher publisher)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _publisher = publisher;
    }
    public async Task<Result> Handle(PowerDeviceWebhookQuery request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetBy(x=> x.DeviceMac == request.Mac);
        if (powerDevice == null)
        {
            _logger.LogWarning("PowerDevice with mac {Mac} not found", request.Mac);
            return Result.Fail(new NotFoundError($"PowerDevice not found"));
        }

        await _publisher.Publish(new DeviceStateChangedEvent(powerDevice.Id, request.state), cancellationToken);
        return Result.Ok();
    }
}

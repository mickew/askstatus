using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.Models;
using Askstatus.Common.PowerDevice;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.DiscoverDevice;

public sealed record DiscoverDevicesQuery() : IRequest<Result<IEnumerable<DicoverInfo>>>;

public sealed class DiscoverDevicesQueryHandler : IRequestHandler<DiscoverDevicesQuery, Result<IEnumerable<DicoverInfo>>>
{
    private readonly ILogger<DiscoverDevicesQueryHandler> _logger;
    private readonly IMqttClientService _mqttClientService;
    private readonly IUnitOfWork _unitOfWork;

    public DiscoverDevicesQueryHandler(ILogger<DiscoverDevicesQueryHandler> logger, IMqttClientService mqttClientService, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _mqttClientService = mqttClientService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<DicoverInfo>>> Handle(DiscoverDevicesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var assignedDevices = await _unitOfWork.PowerDeviceRepository.ListAllAsync();
            var devices = await _mqttClientService.GetShellieDevicesAsync();

            var resultList = new List<DicoverInfo>();

            foreach (var device in devices)
            {
                if (assignedDevices.Any(x => x.DeviceId == device.Id))
                {
                    continue;
                }
                resultList.Add(new DicoverInfo(device.Ip, (PowerDeviceTypes)device.Gen, device.Name, device.Id, device.Mac, device.Model, 0));
            }
            return Result.Ok(resultList.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get devices");
        }
        return Result.Fail(new ServerError("Failed to get devices"));
    }
}

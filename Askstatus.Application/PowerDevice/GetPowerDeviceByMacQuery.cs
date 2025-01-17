using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.PowerDevice;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record GetPowerDeviceByMacQuery(string Mac) : IRequest<Result<PowerDeviceDto>>;

public sealed class GetPowerDeviceByMacQueryHandler : IRequestHandler<GetPowerDeviceByMacQuery, Result<PowerDeviceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPowerDeviceByMacQueryHandler> _logger;
    public GetPowerDeviceByMacQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPowerDeviceByMacQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<PowerDeviceDto>> Handle(GetPowerDeviceByMacQuery request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetBy(x => x.DeviceMac == request.Mac);
        if (powerDevice == null)
        {
            _logger.LogWarning("PowerDevice with mac {Mac} not found", request.Mac);
            return Result.Fail<PowerDeviceDto>(new NotFoundError($"PowerDevice not found"));
        }
        var result = new PowerDeviceDto(powerDevice.Id, powerDevice.Name, powerDevice.DeviceType, powerDevice.HostName, powerDevice.DeviceName, powerDevice.DeviceId, powerDevice.DeviceMac, powerDevice.DeviceModel, powerDevice.DeviceGen);
        return Result.Ok(result);
    }
}

using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.PowerDevice;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record GetPowerDeviceByIdQuery(int Id) : IRequest<Result<PowerDeviceDto>>;

public sealed class GetPowerDeviceByIdQueryHandler : IRequestHandler<GetPowerDeviceByIdQuery, Result<PowerDeviceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPowerDeviceByIdQueryHandler> _logger;

    public GetPowerDeviceByIdQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPowerDeviceByIdQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<PowerDeviceDto>> Handle(GetPowerDeviceByIdQuery request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetByIdAsync(request.Id);
        if (powerDevice == null)
        {
            _logger.LogWarning("PowerDevice with id {Id} not found", request.Id);
            return Result.Fail<PowerDeviceDto>(new NotFoundError($"PowerDevice not found"));
        }
        var result = new PowerDeviceDto(powerDevice.Id, powerDevice.Name, powerDevice.DeviceType, powerDevice.HostName, powerDevice.DeviceName, powerDevice.DeviceId, powerDevice.DeviceMac, powerDevice.DeviceModel, powerDevice.Channel);
        return Result.Ok(result);
    }
}

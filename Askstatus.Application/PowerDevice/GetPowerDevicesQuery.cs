using Askstatus.Application.Interfaces;
using Askstatus.Common.PowerDevice;
using FluentResults;
using MediatR;

namespace Askstatus.Application.PowerDevice;
public sealed record GetPowerDevicesQuery : IRequest<Result<IEnumerable<PowerDeviceDto>>>;

public sealed class GetPowerDevicesQueryHandler : IRequestHandler<GetPowerDevicesQuery, Result<IEnumerable<PowerDeviceDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPowerDevicesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<PowerDeviceDto>>> Handle(GetPowerDevicesQuery request, CancellationToken cancellationToken)
    {
        //throw new NotImplementedException();
        var powerDevices = await _unitOfWork.PowerDeviceRepository.ListAllAsync();
        var result = powerDevices.Select(x => new PowerDeviceDto(x.Id, x.Name, x.DeviceType, x.HostName, x.DeviceName, x.DeviceId, x.DeviceMac, x.DeviceModel, x.Channel, x.ChanelType));
        return Result.Ok(result);
    }
}

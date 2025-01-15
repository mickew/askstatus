using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record GetPowerDeviceStateQuery(int id) : IRequest<Result<bool>>;

public sealed class GetPowerDeviceStateQueryHandler : IRequestHandler<GetPowerDeviceStateQuery, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetPowerDeviceStateQueryHandler> _logger;
    private readonly IDeviceService _deviceService;
    public GetPowerDeviceStateQueryHandler(IUnitOfWork unitOfWork, ILogger<GetPowerDeviceStateQueryHandler> logger, IDeviceService deviceService)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _deviceService = deviceService;
    }
    public async Task<Result<bool>> Handle(GetPowerDeviceStateQuery request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetByIdAsync(request.id);
        if (powerDevice is null)
        {
            _logger.LogError("PowerDevice with Id: {id} not found", request.id);
            return Result.Fail<bool>(new NotFoundError("PowerDevice not found"));
        }
        else
        {
            var result = await _deviceService.State(powerDevice.HostName, 0);
            return result;
        }
    }
}

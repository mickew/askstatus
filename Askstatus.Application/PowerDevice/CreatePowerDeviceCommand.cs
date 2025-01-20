using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.PowerDevice;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record CreatePowerDeviceCommand : IRequest<Result<PowerDeviceDto>>
{
    public string Name { get; init; } = null!;
    public PowerDeviceTypes DeviceType { get; init; }
    public string HostName { get; init; } = null!;
    public string DeviceName { get; init; } = null!;
    public string DeviceId { get; init; } = null!;
    public string DeviceMac { get; init; } = null!;
    public string DeviceModel { get; init; } = null!;
    public int Channel { get; init; }
}

public sealed class CreatePowerDeviceCommandHandler : IRequestHandler<CreatePowerDeviceCommand, Result<PowerDeviceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreatePowerDeviceCommandHandler> _logger;
    public CreatePowerDeviceCommandHandler(IUnitOfWork unitOfWork, ILogger<CreatePowerDeviceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<PowerDeviceDto>> Handle(CreatePowerDeviceCommand request, CancellationToken cancellationToken)
    {
        var powerDevice = new Askstatus.Domain.Entities.PowerDevice
        {
            Name = request.Name,
            DeviceType = request.DeviceType,
            HostName = request.HostName,
            DeviceName = request.DeviceName,
            DeviceId = request.DeviceId,
            DeviceMac = request.DeviceMac,
            DeviceModel = request.DeviceModel,
            Channel = request.Channel
        };
        var result = await _unitOfWork.PowerDeviceRepository.AddAsync(powerDevice);
        if (result is null)
        {
            _logger.LogError("Error creating PowerDevice");
            return Result.Fail<PowerDeviceDto>(new BadRequestError("Error creating PowerDevice"));
        }
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok(new PowerDeviceDto(result.Id, result.Name, result.DeviceType, result.HostName, result.DeviceName, result.DeviceId, result.DeviceMac, result.DeviceModel, result.Channel));
    }
}

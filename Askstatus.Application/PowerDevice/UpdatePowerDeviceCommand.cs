using Askstatus.Application.Errors;
using Askstatus.Application.Interfaces;
using Askstatus.Common.PowerDevice;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Askstatus.Application.PowerDevice;
public sealed record UpdatePowerDeviceCommand : IRequest<Result>
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public PowerDeviceTypes DeviceType { get; init; }
    public string? HostName { get; init; }
    public string? DeviceName { get; init; }
    public string? DeviceId { get; init; }
    public string? DeviceMac { get; init; }
    public string? DeviceModel { get; init; }
    public int Channel { get; init; }
    public ChanelType ChanelType { get; init; }
}

public sealed class UpdatePowerDeviceCommandHandler : IRequestHandler<UpdatePowerDeviceCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdatePowerDeviceCommandHandler> _logger;

    public UpdatePowerDeviceCommandHandler(IUnitOfWork unitOfWork, ILogger<UpdatePowerDeviceCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result> Handle(UpdatePowerDeviceCommand request, CancellationToken cancellationToken)
    {
        var powerDevice = await _unitOfWork.PowerDeviceRepository.GetByIdAsync(request.Id);
        if (powerDevice == null)
        {
            _logger.LogWarning("PowerDevice with id {Id} not found", request.Id);
            return Result.Fail(new NotFoundError($"PowerDevice not found"));
        }
        powerDevice.Name = request.Name!;
        powerDevice.DeviceType = request.DeviceType;
        powerDevice.HostName = request.HostName!;
        powerDevice.DeviceName = request.DeviceName!;
        powerDevice.DeviceId = request.DeviceId!;
        powerDevice.DeviceMac = request.DeviceMac!;
        powerDevice.DeviceModel = request.DeviceModel!;
        powerDevice.Channel = request.Channel;
        powerDevice.ChanelType = request.ChanelType;
        var result = await _unitOfWork.PowerDeviceRepository.UpdateAsync(powerDevice);
        if (!result)
        {
            _logger.LogError("Error updating PowerDevice with id {Id}", request.Id);
            return Result.Fail(new BadRequestError("Error updating PowerDevice"));
        }
        var ret = await _unitOfWork.SaveChangesAsync();
        if (ret == -1)
        {
            _logger.LogError("Error saving changes");
            return Result.Fail(new BadRequestError("Error saving changes"));
        }
        return Result.Ok();
    }
}

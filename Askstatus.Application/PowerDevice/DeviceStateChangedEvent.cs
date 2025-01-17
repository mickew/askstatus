using MediatR;

namespace Askstatus.Application.PowerDevice;
public record DeviceStateChangedEvent(int Id, bool State) : INotification;

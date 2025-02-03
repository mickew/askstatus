using Askstatus.Application.Events;

namespace Askstatus.Application.PowerDevice;
public record DeviceStateChangedIntegrationEvent(Guid Id, int DeviceId, bool State) : IntegrationEvent(Id);

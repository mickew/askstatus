using Askstatus.Application.Events;

namespace Askstatus.Application.Sensors;
public record SensorValueChangedIntegrationEvent(Guid Id, int SensorId, string NewValue, DateTime TimeStamp) : IntegrationEvent(Id);

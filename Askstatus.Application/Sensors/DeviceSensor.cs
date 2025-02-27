namespace Askstatus.Application.Sensors;

public sealed record DeviceSensor(string Id, List<DeviceSensorValue> Values);


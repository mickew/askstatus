namespace Askstatus.Application.Sensors;

public sealed record DeviceSensor(string Id, string Name, string Model, List<DeviceSensorValue> Values);


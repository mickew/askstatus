namespace Askstatus.Common.Sensor;
public sealed record SensorRequest(int Id, string Name, SensorType SensorType, string FormatString, string SensorName, string SensorModel, string ValueName);

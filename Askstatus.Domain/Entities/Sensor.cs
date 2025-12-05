using Askstatus.Common.Sensor;

namespace Askstatus.Domain.Entities;

public class Sensor
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public SensorType SensorType { get; set; }
    public string FormatString { get; set; } = null!;

    public string SensorName { get; set; } = null!;
    public string ValueName { get; set; } = null!;
}

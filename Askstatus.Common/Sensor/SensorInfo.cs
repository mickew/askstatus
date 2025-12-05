namespace Askstatus.Common.Sensor;
public class SensorInfo
{
    public string Id { get; set; }

    public List<SensorValue> Values { get; set; }

    public SensorInfo(string id, List<SensorValue> values)
    {
        Id = id;
        Values = values;
    }
}

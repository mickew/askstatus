namespace Askstatus.Common.Sensor;
public class SensorInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Model { get; set; }

    public List<SensorValue> Values { get; set; }

    public SensorInfo(string id, string name, string model, List<SensorValue> values)
    {
        Id = id;
        Name = name;
        Model = model;
        Values = values;
    }
}

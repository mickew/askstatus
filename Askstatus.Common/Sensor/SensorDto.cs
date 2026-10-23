namespace Askstatus.Common.Sensor;
public sealed class SensorDto
{
    public SensorDto() : this(0, string.Empty, SensorType.Unknown, string.Empty, string.Empty, string.Empty, string.Empty, false) { }
    public SensorDto(SensorDto sensorDto) : this(sensorDto.Id, sensorDto.Name, sensorDto.SensorType, sensorDto.FormatString, sensorDto.SensorName, sensorDto.SensorModel, sensorDto.ValueName, sensorDto.MQTTEnabled) { }
    public SensorDto(int id, string name, SensorType type, string formatString, string sensorName, string sensorModel, string valueName, bool mqttEnabled)
    {
        Id = id;
        Name = name;
        SensorType = type;
        FormatString = formatString;
        SensorName = sensorName;
        SensorModel = sensorModel;
        ValueName = valueName;
        MQTTEnabled = mqttEnabled;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public SensorType SensorType { get; set; }
    public string FormatString { get; set; }
    public string SensorName { get; set; }
    public string SensorModel { get; set; }
    public string ValueName { get; set; }
    public bool MQTTEnabled { get; set; } = false;
}

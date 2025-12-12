namespace Askstatus.Common.Sensor;
public sealed class SensorDto
{
    public SensorDto() : this(0, string.Empty, SensorType.Unknown, string.Empty, string.Empty, string.Empty, string.Empty) { }
    public SensorDto(SensorDto sensorDto) : this(sensorDto.Id, sensorDto.Name, sensorDto.SensorType, sensorDto.FormatString, sensorDto.SensorName, sensorDto.SensorModel, sensorDto.ValueName) { }
    public SensorDto(int id, string name, SensorType type, string formatString, string sensorName, string sensorModel, string valueName)
    {
        Id = id;
        Name = name;
        SensorType = type;
        FormatString = formatString;
        SensorName = sensorName;
        SensorModel = sensorModel;
        ValueName = valueName;
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public SensorType SensorType { get; set; }
    public string FormatString { get; set; }
    public string SensorName { get; set; }
    public string SensorModel { get; set; }
    public string ValueName { get; set; }
}

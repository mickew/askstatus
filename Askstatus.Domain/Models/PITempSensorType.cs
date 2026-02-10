using System.Runtime.Serialization;

namespace Askstatus.Domain.Models;

[DataContract]
public enum PITempSensorType
{
    [EnumMember(Value = "DS18B20")]
    DS18B20
}

using System.Runtime.Serialization;

namespace Askstatus.Domain.Models;

[DataContract]

public enum PINutSensorType
{
    [EnumMember(Value = "PINUT")]
    PINUT
}

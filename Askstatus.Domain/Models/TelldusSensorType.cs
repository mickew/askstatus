using System.Runtime.Serialization;

namespace Askstatus.Domain.Models;

[DataContract]
public enum TelldusSensorType
{
    [EnumMember(Value = "TELLDUS")]
    TELLDUS,

}

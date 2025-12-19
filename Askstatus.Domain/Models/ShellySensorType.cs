using System.Runtime.Serialization;

namespace Askstatus.Domain.Models;
[DataContract]
public enum ShellySensorType
{
    [EnumMember(Value = "SHHT-1")]
    SHHT_1,
    [EnumMember(Value = "S3SN-0U12A")]
    S3SN_0U12A
}

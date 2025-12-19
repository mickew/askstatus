using Askstatus.Domain.Models;
using Askstatus.Domain.Extensions;

namespace Askstatus.Domain.Constants;
public static class SuportedShellySensorTypes
{

    public static readonly string[] Sensors = new[]
    {
        EnumExtensions.GetEnumValue(ShellySensorType.SHHT_1),
        EnumExtensions.GetEnumValue<ShellySensorType>(ShellySensorType.S3SN_0U12A)
    };
}

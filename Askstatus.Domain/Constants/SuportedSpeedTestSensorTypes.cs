using Askstatus.Domain.Extensions;
using Askstatus.Domain.Models;

namespace Askstatus.Domain.Constants;

public class SupportedSpeedTestSensorTypes
{
    public static readonly string[] Sensors = new[]
    {
        EnumExtensions.GetEnumValue(SpeedTestSensorType.SPEEDTEST)
    };
}

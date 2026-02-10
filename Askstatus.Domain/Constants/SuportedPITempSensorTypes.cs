using Askstatus.Domain.Extensions;
using Askstatus.Domain.Models;

namespace Askstatus.Domain.Constants;

public class SuportedPITempSensorTypes
{
    public static readonly string[] Sensors = new[]
    {
        EnumExtensions.GetEnumValue(PITempSensorType.DS18B20)
    };
}

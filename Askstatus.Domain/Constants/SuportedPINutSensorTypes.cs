using Askstatus.Domain.Extensions;
using Askstatus.Domain.Models;

namespace Askstatus.Domain.Constants;

public class SuportedPINutSensorTypes
{
    public static readonly string[] Sensors = new[]
    {
        EnumExtensions.GetEnumValue(PINutSensorType.PINUT)
    };
}

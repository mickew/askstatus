using Askstatus.Domain.Extensions;
using Askstatus.Domain.Models;

namespace Askstatus.Domain.Constants;

public class SuportedTelldusSensorTypes
{
    public static readonly string[] Sensors = new[]
    {
        EnumExtensions.GetEnumValue(TelldusSensorType.TELLDUS)
    };
}

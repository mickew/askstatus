using System.Globalization;
using Askstatus.Domain.Entities;
using Askstatus.Domain.Extensions;
using Askstatus.Domain.Models;

namespace Askstatus.Application.Sensors;
public static class ParseSensor
{
    public static bool TryParseValue(string value, Sensor sensor, out double result)
    {
        double valueDouble = default;
        try
        {
            bool isDouble;
            var model = EnumExtensions.GetEnumFromString<ShellySensorType>(sensor.SensorModel);
            switch (model)
            {
                case ShellySensorType.SHHT_1:
                    isDouble = double.TryParse(value, CultureInfo.InvariantCulture, out valueDouble);
                    break;
                case ShellySensorType.S3SN_0U12A:
                    isDouble = ShellyS3SN_0U12A.TryParse(value, out valueDouble);
                    break;
                default:
                    isDouble = false;
                    break;
            }
            result = valueDouble;
            return isDouble;
        }
        catch (Exception)
        {
            result = valueDouble;
            return false;
        }
    }
}

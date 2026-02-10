using System.Globalization;
using Askstatus.Domain.Constants;
using Askstatus.Domain.Entities;
using Askstatus.Domain.Extensions;
using Askstatus.Domain.Models;

namespace Askstatus.Application.Sensors;
public static class ParseSensor
{
    public static bool TryParseValue(string value, Sensor sensor, out double result)
    {
        if (SuportedShellySensorTypes.Sensors.Contains(sensor.SensorModel))
        {
            return TryParseShellySensorType(value, sensor.SensorModel, out result);
        }
        else if (SuportedPITempSensorTypes.Sensors.Contains(sensor.SensorModel))
        {
            return TryParsePITempSensorType(value, sensor.SensorModel, out result);
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static bool TryParseShellySensorType(string value, String sensorModel, out double result)
    {
        double valueDouble = default;
        try
        {
            bool isDouble;
            var model = EnumExtensions.GetEnumFromString<ShellySensorType>(sensorModel);
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

    public static bool TryParsePITempSensorType(string value, String sensorModel, out double result)
    {
        double valueDouble = default;
        try
        {
            bool isDouble;
            var model = EnumExtensions.GetEnumFromString<PITempSensorType>(sensorModel);
            switch (model)
            {
                case PITempSensorType.DS18B20:
                    isDouble = double.TryParse(value, CultureInfo.InvariantCulture, out valueDouble);
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

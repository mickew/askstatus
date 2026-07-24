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
        else if (SuportedPINutSensorTypes.Sensors.Contains(sensor.SensorModel))
        {
            return TryParsePINutSensorType(value, sensor.SensorModel, out result);
        }
        else if (SuportedTelldusSensorTypes.Sensors.Contains(sensor.SensorModel))
        {
            return TryParseTelldusSensorType(value, sensor.SensorModel, out result);
        }
        else
        {
            result = default;
            return false;
        }
    }

    public static bool TryFormatValue(double value, string formatString, out string result)
    {

        if (!string.IsNullOrEmpty(formatString))
        {
            if (formatString.StartsWith(FormatTypes.WindSpeed))
            {
                var dir = WindDirectionConverter.DegreesToDirection(value);
                result = string.Format(CultureInfo.InvariantCulture, formatString.Replace(FormatTypes.WindSpeed, "{0}"), dir);
                return true;
            }
        
            result = string.Format(CultureInfo.InvariantCulture,formatString, value);
            return true;
        }
        result = string.Empty;
        return false;
    }

    private static bool TryParseShellySensorType(string value, String sensorModel, out double result)
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

    private static bool TryParsePITempSensorType(string value, String sensorModel, out double result)
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

    private static bool TryParsePINutSensorType(string value, String sensorModel, out double result)
    {
        double valueDouble = default;
        try
        {
            bool isDouble;
            var model = EnumExtensions.GetEnumFromString<PINutSensorType>(sensorModel);
            switch (model)
            {
                case PINutSensorType.PINUT:
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

    private static bool TryParseTelldusSensorType(string value, string sensorModel, out double result)
    {
        double valueDouble = default;
        try
        {
            bool isDouble;
            var model = EnumExtensions.GetEnumFromString<TelldusSensorType>(sensorModel);
            switch (model)
            {
                case TelldusSensorType.TELLDUS:
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

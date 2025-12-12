using Askstatus.Common.PowerDevice;
using Askstatus.Common.Sensor;

namespace Askstatus.Web.App.Layout;

public static class AskstatusIcons
{
    public const string HeatOn = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M7.95,3L6.53,5.19L7.95,7.4H7.94L5.95,10.5L4.22,9.6L5.64,7.39L4.22,5.19L6.22,2.09L7.95,3M13.95,2.89L12.53,5.1L13.95,7.3L13.94,7.31L11.95,10.4L10.22,9.5L11.64,7.3L10.22,5.1L12.22,2L13.95,2.89M20,2.89L18.56,5.1L20,7.3V7.31L18,10.4L16.25,9.5L17.67,7.3L16.25,5.1L18.25,2L20,2.89M2,22V14A2,2 0 0,1 4,12H20A2,2 0 0,1 22,14V22H20V20H4V22H2M6,14A1,1 0 0,0 5,15V17A1,1 0 0,0 6,18A1,1 0 0,0 7,17V15A1,1 0 0,0 6,14M10,14A1,1 0 0,0 9,15V17A1,1 0 0,0 10,18A1,1 0 0,0 11,17V15A1,1 0 0,0 10,14M14,14A1,1 0 0,0 13,15V17A1,1 0 0,0 14,18A1,1 0 0,0 15,17V15A1,1 0 0,0 14,14M18,14A1,1 0 0,0 17,15V17A1,1 0 0,0 18,18A1,1 0 0,0 19,17V15A1,1 0 0,0 18,14Z\"/></svg>";

    public const string HeatOff = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M3.28,2L2,3.27L4.77,6.04L5.64,7.39L4.22,9.6L5.95,10.5L7.23,8.5L10.73,12H4A2,2 0 0,0 2,14V22H4V20H18.73L20,21.27V22H22V20.73L22,20.72V20.72L3.28,2M7,17A1,1 0 0,1 6,18A1,1 0 0,1 5,17V15A1,1 0 0,1 6,14A1,1 0 0,1 7,15V17M11,17A1,1 0 0,1 10,18A1,1 0 0,1 9,17V15A1,1 0 0,1 10,14A1,1 0 0,1 11,15V17M15,17A1,1 0 0,1 14,18A1,1 0 0,1 13,17V15C13,14.79 13.08,14.61 13.18,14.45L15,16.27V17M16.25,9.5L17.67,7.3L16.25,5.1L18.25,2L20,2.89L18.56,5.1L20,7.3V7.31L18,10.4L16.25,9.5M22,14V18.18L19,15.18V15A1,1 0 0,0 18,14C17.95,14 17.9,14 17.85,14.03L15.82,12H20C21.11,12 22,12.9 22,14M11.64,7.3L10.22,5.1L12.22,2L13.95,2.89L12.53,5.1L13.95,7.3L13.94,7.31L12.84,9L11.44,7.62L11.64,7.3M7.5,3.69L6.1,2.28L6.22,2.09L7.95,3L7.5,3.69Z\"/></svg>";

    public const string RelayOn = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M7,2V13H10V22L17,10H13L17,2H7Z\"/></svg>";

    public const string RelayOff = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M17,10H13L17,2H7V4.18L15.46,12.64M3.27,3L2,4.27L7,9.27V13H10V22L13.58,15.86L17.73,20L19,18.73L3.27,3Z\"/></svg>";

    public const string BulbOn = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M12,2A7,7 0 0,0 5,9C5,11.38 6.19,13.47 8,14.74V17A1,1 0 0,0 9,18H15A1,1 0 0,0 16,17V14.74C17.81,13.47 19,11.38 19,9A7,7 0 0,0 12,2M9,21A1,1 0 0,0 10,22H14A1,1 0 0,0 15,21V20H9V21Z\"/></svg>";

    public const string BulbOff = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M12,2C9.76,2 7.78,3.05 6.5,4.68L16.31,14.5C17.94,13.21 19,11.24 19,9A7,7 0 0,0 12,2M3.28,4L2,5.27L5.04,8.3C5,8.53 5,8.76 5,9C5,11.38 6.19,13.47 8,14.74V17A1,1 0 0,0 9,18H14.73L18.73,22L20,20.72L3.28,4M9,20V21A1,1 0 0,0 10,22H14A1,1 0 0,0 15,21V20H9Z\"/></svg>";

    public const string GenericOn = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M16,7V3H14V7H10V3H8V7H8C7,7 6,8 6,9V14.5L9.5,18V21H14.5V18L18,14.5V9C18,8 17,7 16,7Z\"/></svg>";

    public const string GenericOff = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M20.84 22.73L15.31 17.2L14.5 18V21H9.5V18L6 14.5V9C6 8.7 6.1 8.41 6.25 8.14L1.11 3L2.39 1.73L22.11 21.46L20.84 22.73M18 14.5V9C18 8 17 7 16 7V3H14V7H10.2L17.85 14.65L18 14.5M10 3H8V4.8L10 6.8V3Z\"/></svg>";

    public const string Thermometer = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M15 13V5A3 3 0 0 0 9 5V13A5 5 0 1 0 15 13M12 4A1 1 0 0 1 13 5V8H11V5A1 1 0 0 1 12 4Z\"/></svg>";

    public const string Humidity = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M12,3.25C12,3.25 6,10 6,14C6,17.32 8.69,20 12,20A6,6 0 0,0 18,14C18,10 12,3.25 12,3.25M14.47,9.97L15.53,11.03L9.53,17.03L8.47,15.97M9.75,10A1.25,1.25 0 0,1 11,11.25A1.25,1.25 0 0,1 9.75,12.5A1.25,1.25 0 0,1 8.5,11.25A1.25,1.25 0 0,1 9.75,10M14.25,14.5A1.25,1.25 0 0,1 15.5,15.75A1.25,1.25 0 0,1 14.25,17A1.25,1.25 0 0,1 13,15.75A1.25,1.25 0 0,1 14.25,14.5Z\"/></svg>";

    public const string Unknown = "<svg style=\"width:24px;height:24px\" viewBox=\"0 0 24 24\"><path fill=\"currentColor\" d=\"M21.86 12.5C21.1 11.63 20.15 11.13 19 11C19 9.05 18.32 7.4 16.96 6.04C15.6 4.68 13.95 4 12 4C10.42 4 9 4.47 7.75 5.43S5.67 7.62 5.25 9.15C4 9.43 2.96 10.08 2.17 11.1S1 13.28 1 14.58C1 16.09 1.54 17.38 2.61 18.43C3.69 19.5 5 20 6.5 20H18.5C19.75 20 20.81 19.56 21.69 18.69C22.56 17.81 23 16.75 23 15.5C23 14.35 22.62 13.35 21.86 12.5M13 17H11V15H13V17M14.8 11.82C14.5 12.21 14.13 12.5 13.67 12.75C13.41 12.91 13.24 13.07 13.15 13.26C13.06 13.45 13 13.69 13 14H11C11 13.45 11.11 13.08 11.3 12.82C11.5 12.56 11.85 12.25 12.37 11.91C12.63 11.75 12.84 11.56 13 11.32C13.15 11.09 13.23 10.81 13.23 10.5C13.23 10.18 13.14 9.94 12.96 9.76C12.78 9.56 12.5 9.47 12.2 9.47C11.93 9.47 11.71 9.55 11.5 9.7C11.35 9.85 11.25 10.08 11.25 10.39H9.28C9.23 9.64 9.5 9 10.06 8.59C10.6 8.2 11.31 8 12.2 8C13.14 8 13.89 8.23 14.43 8.68S15.24 9.75 15.24 10.5C15.24 11 15.09 11.41 14.8 11.82Z\"/></svg>";


    public static string SensorTypeToIcon(SensorType sensorType)
    {
        return sensorType switch
        {
            SensorType.Temperature => AskstatusIcons.Thermometer,
            SensorType.Humidity => AskstatusIcons.Humidity,
            _ => AskstatusIcons.Unknown,
        };
    }

    public static string ChanelTypeToIcon(ChanelType chanelType)
    {
        return chanelType switch
        {
            ChanelType.Relay => RelayOn,
            ChanelType.Heat => HeatOn,
            ChanelType.Bulb => BulbOn,
            _ => GenericOn,
        };
    }

    public static string ChanelTypeStateToIcon(ChanelType chanelType, bool state)
    {
        return chanelType switch
        {
            ChanelType.Relay => state ? RelayOn : RelayOff,
            ChanelType.Heat => state ? HeatOn : HeatOff,
            ChanelType.Bulb => state ? BulbOn : BulbOff,
            _ => state ? GenericOn : GenericOff,
        };
    }
}

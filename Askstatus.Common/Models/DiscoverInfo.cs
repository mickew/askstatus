using Askstatus.Common.PowerDevice;

namespace Askstatus.Common.Models;
public class DicoverInfo
{
    public string DeviceHostName { get; set; }
    public PowerDeviceTypes DeviceType { get; set; }
    public string DeviceName { get; set; }
    public string DeviceId { get; set; }
    public string DeviceMac { get; set; }
    public string DeviceModel { get; set; }
    public int Channel { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public DicoverInfo() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public DicoverInfo(
        string deviceHostName,
        PowerDeviceTypes deviceType,
        string deviceName,
        string deviceId,
        string deviceMac,
        string deviceModel,
        int channel)
    {
        DeviceHostName = deviceHostName;
        DeviceType = deviceType;
        DeviceName = deviceName;
        DeviceId = deviceId;
        DeviceMac = deviceMac;
        DeviceModel = deviceModel;
        Channel = channel;
    }
}

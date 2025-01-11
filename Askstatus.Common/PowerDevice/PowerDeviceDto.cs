namespace Askstatus.Common.PowerDevice;
public class PowerDeviceDto
{
    public PowerDeviceDto() : this(0, string.Empty, PowerDeviceTypes.Generic, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0) { }

    public PowerDeviceDto(int id, string name, PowerDeviceTypes deviceType, string hostName, string deviceName, string deviceId, string deviceMac, string deviceModel, int deviceGen)
    {
        ID = id;
        Name = name;
        DeviceType = deviceType;
        HostName = hostName;
        DeviceName = deviceName;
        DeviceId = deviceId;
        DeviceMac = deviceMac;
        DeviceModel = deviceModel;
        DeviceGen = deviceGen;
    }
    public int ID { get; set; }
    public string Name { get; set; }
    public PowerDeviceTypes DeviceType { get; set; }
    public string HostName { get; set; }
    public string DeviceName { get; set; }
    public string DeviceId { get; set; }
    public string DeviceMac { get; set; }
    public string DeviceModel { get; set; }
    public int DeviceGen { get; set; }
}

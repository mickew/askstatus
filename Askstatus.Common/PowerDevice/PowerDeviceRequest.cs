namespace Askstatus.Common.PowerDevice;
public sealed record PowerDeviceRequest(int Id, string Name, PowerDeviceTypes DeviceType, string HostName, string DeviceName, string DeviceId, string DeviceMac, string DeviceModel, int Channel);


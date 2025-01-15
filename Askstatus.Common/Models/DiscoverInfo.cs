using Askstatus.Common.PowerDevice;

namespace Askstatus.Common.Models;
public record DicoverInfo(string DeviceHostName, PowerDeviceTypes DeviceType, string DeviceName, string DeviceId, string DeviceMac, string DeviceModel, int DeviceGen);

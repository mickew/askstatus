using Askstatus.Common.PowerDevice;

namespace Askstatus.Domain.Entities;
public class PowerDevice
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public PowerDeviceTypes DeviceType { get; set; }
    public string HostName { get; set; } = null!;
    public string DeviceName { get; set; } = null!;
    public string DeviceId { get; set; } = null!;
    public string DeviceMac { get; set; } = null!;
    public string DeviceModel { get; set; } = null!;
    public int DeviceGen { get; set; }
}

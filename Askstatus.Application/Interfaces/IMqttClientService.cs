using Askstatus.Application.DiscoverDevice;
using Askstatus.Application.Sensors;
using Microsoft.Extensions.Hosting;

namespace Askstatus.Application.Interfaces;

public interface IMqttClientService : IHostedService
{
    Task<IEnumerable<ShellieAnnounce>> GetShellieDevicesAsync();
    Task RefreshShellieDevicesAsync();
    Task RefreshStatusAsync();
    Task<IEnumerable<DeviceSensor>> GetSensorsAsync();
    Task<bool> ToggleDeviceAsync(string deviceId, int switchId);
    Task<bool> SwitchDeviceAsync(string deviceId, int switchId, bool state);
}

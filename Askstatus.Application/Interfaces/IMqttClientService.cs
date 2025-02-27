using Askstatus.Application.DiscoverDevice;
using Askstatus.Application.Sensors;
using Microsoft.Extensions.Hosting;

namespace Askstatus.Application.Interfaces;

public interface IMqttClientService : IHostedService
{
    Task<IEnumerable<ShellieAnnounce>> GetShellieDevicesAsync();
    Task RefreshShellieDevicesAsync();
    Task<IEnumerable<DeviceSensor>> GetSensorsAsync();
}

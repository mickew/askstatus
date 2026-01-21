using Askstatus.Common.Models;
using Askstatus.Common.PowerDevice;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Devices;

public partial class Index
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private NavigationManager? Navigation { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    protected bool UserGotNoRights { get; set; } = true;

    public List<PowerDeviceDto> PowerDevices { get; set; } = new List<PowerDeviceDto>();

    protected override async Task OnInitializedAsync()
    {
        var res = await ApiService.PowerDeviceAPI.Refresh();
        if (!res.IsSuccessStatusCode)
        {
            Logger.LogError(res.Error, res.Error.Content);
            Snackbar.Add(res.Error.Content!, Severity.Error);
            return;
        }

        var response = await ApiService.PowerDeviceAPI.GetPowerDevices();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        PowerDevices = response.Content!.ToList();
    }

    private async Task EditDevice(PowerDeviceDto device)
    {
        var deviceCopy = new PowerDeviceDto(device);
        var parameters = new DialogParameters<EditDeviceDialog> { { x => x.device, deviceCopy } };

        var dialog = await DialogService.ShowAsync<EditDeviceDialog>("Edit Device", parameters);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            if (result.Data is PowerDeviceDto)
            {
                deviceCopy = ((PowerDeviceDto)result.Data);
                PowerDeviceRequest powerDeviceRequest = new(deviceCopy.Id, deviceCopy.Name, deviceCopy.DeviceType, deviceCopy.HostName, deviceCopy.DeviceName, deviceCopy.DeviceId, deviceCopy.DeviceMac, deviceCopy.DeviceModel, deviceCopy.Channel, deviceCopy.ChanelType);
                var res = await ApiService.PowerDeviceAPI.UpdatePowerDevice(powerDeviceRequest);
                if (!res.IsSuccessStatusCode)
                {
                    Logger.LogError(res.Error, res.Error.Content);
                    Snackbar.Add(res.Error.Content!, Severity.Error);
                    return;
                }
                device.Name = deviceCopy.Name;
                device.ChanelType = deviceCopy.ChanelType;
                Snackbar.Add($"Device {device.Name} updated", Severity.Success);
                StateHasChanged();
            }
        }
    }

    private async Task DeleteDevice(PowerDeviceDto device)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete device {device.Name} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            var response = await ApiService.PowerDeviceAPI.DeletePowerDevice(device.Id);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError(response.Error, response.Error.Content);
                Snackbar.Add(response.Error.Content!, Severity.Error);
                return;
            }
            PowerDevices.Remove(device);
            StateHasChanged();
            Snackbar.Add($"Device {device.Name} deleted", Severity.Success);
        }
    }

    private async Task DiscoverDevice()
    {
        var parameters = new DialogParameters<DiscoverDeviceDialog> { { x => x.Device, new PowerDeviceDto() } };

        var dialog = await DialogService.ShowAsync<DiscoverDeviceDialog>("Discover device", parameters);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            if (result.Data is PowerDeviceDto)
            {
                var device = ((PowerDeviceDto)result.Data);
                PowerDeviceRequest powerDeviceRequest = new(device.Id, device.Name, device.DeviceType, device.HostName, device.DeviceName, device.DeviceId, device.DeviceMac, device.DeviceModel, device.Channel, device.ChanelType);
                var res = await ApiService.PowerDeviceAPI.CreatePowerDevice(powerDeviceRequest);
                if (!res.IsSuccessStatusCode)
                {
                    Logger.LogError(res.Error, res.Error.Content);
                    Snackbar.Add(res.Error.Content!, Severity.Error);
                    return;
                }
                PowerDevices.Add(res.Content!);
                Snackbar.Add($"Device {res.Content!.Name} added", Severity.Success);
            }
        }
    }

    private async Task DiscoverAllDevices()
    {
        var response = await ApiService.DeviceDiscoverAPI.DiscoverAll();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        await HandleDiscoverDevicesDialog(response.Content!);
    }

    private async Task DiscoverDevices()
    {
        var response = await ApiService.DeviceDiscoverAPI.NotAssigned();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        await HandleDiscoverDevicesDialog(response.Content!);
    }

    private async Task HandleDiscoverDevicesDialog(IEnumerable<DicoverInfo> dicoverInfos)
    {
        var parameters = new DialogParameters<DiscoverAllDevicesDialog> { { x => x.DicoverInfos, dicoverInfos } };
        DialogOptions dialogOptions = new() { MaxWidth = MaxWidth.Medium, FullWidth = true };

        var dialog = await DialogService.ShowAsync<DiscoverAllDevicesDialog>("Discover devices", parameters, dialogOptions);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            if (result.Data is DicoverInfo && result.Data is not null)
            {
                var selectedDevice = (DicoverInfo)result.Data;
                PowerDeviceDto device = new PowerDeviceDto
                {
                    Name = selectedDevice.DeviceName,
                    DeviceType = selectedDevice.DeviceType,
                    HostName = selectedDevice.DeviceHostName,
                    DeviceName = selectedDevice.DeviceName,
                    DeviceId = selectedDevice.DeviceId,
                    DeviceMac = selectedDevice.DeviceMac,
                    DeviceModel = selectedDevice.DeviceModel,
                    Channel = selectedDevice.Channel,
                    ChanelType = ChanelType.Generic
                };
                PowerDeviceRequest powerDeviceRequest = new(0, device.Name, device.DeviceType, device.HostName, device.DeviceName, device.DeviceId, device.DeviceMac, device.DeviceModel, device.Channel, device.ChanelType);
                var res = await ApiService.PowerDeviceAPI.CreatePowerDevice(powerDeviceRequest);
                if (!res.IsSuccessStatusCode)
                {
                    Logger.LogError(res.Error, res.Error.Content);
                    Snackbar.Add(res.Error.Content!, Severity.Error);
                    return;
                }
                PowerDevices.Add(res.Content!);
                Snackbar.Add($"Device {res.Content!.Name} added", Severity.Success);
            }
        }
    }
}

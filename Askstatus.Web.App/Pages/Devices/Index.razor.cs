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
        var parameters = new DialogParameters<EditDeviceDialog> { { x => x.device, device } };

        var dialog = await DialogService.ShowAsync<EditDeviceDialog>("Edit Device", parameters);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            if (result.Data is PowerDeviceDto)
            {
                device = ((PowerDeviceDto)result.Data);
                PowerDeviceRequest powerDeviceRequest = new(device.Id, device.Name, device.DeviceType, device.HostName, device.DeviceName, device.DeviceId, device.DeviceMac, device.DeviceModel, device.Channel);
                var res = await ApiService.PowerDeviceAPI.UpdatePowerDevice(powerDeviceRequest);
                if (!res.IsSuccessStatusCode)
                {
                    Logger.LogError(res.Error, res.Error.Content);
                    Snackbar.Add(res.Error.Content!, Severity.Error);
                    return;
                }
                Snackbar.Add($"Device {device.Name} updated", Severity.Success);
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
                PowerDeviceRequest powerDeviceRequest = new(device.Id, device.Name, device.DeviceType, device.HostName, device.DeviceName, device.DeviceId, device.DeviceMac, device.DeviceModel, device.Channel);
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

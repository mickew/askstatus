using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages;

public partial class Home
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    public List<Device> Devices { get; set; } = new List<Device>();

    protected bool UserGotNoRights { get; set; } = true;

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiService.PowerDeviceAPI.GetPowerDevices();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        Devices = response.Content!.Select(x => new Device
        {
            Id = x.Id,
            Mac = x.DeviceMac,
            Name = x.DeviceName,
        }).ToList();
        foreach (var device in Devices)
        {
            var stateResponse = await ApiService.PowerDeviceAPI.GetPowerDeviceStatus(device.Id);
            if (!stateResponse.IsSuccessStatusCode)
            {
                Logger.LogError(stateResponse.Error, stateResponse.Error.Content);
                Snackbar.Add(stateResponse.Error.Content!, Severity.Error);
                return;
            }
            device.State = stateResponse.Content!;
        }
    }

    private async Task ToggleDevice(int id)
    {
        var device = Devices.Where(x => x.Id == id).FirstOrDefault();
        device!.Prosessing = true;
        StateHasChanged();
        var response = await ApiService.PowerDeviceAPI.TogglePowerDevice(id);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            device.Prosessing = false;
            StateHasChanged();
            return;
        }
        await Task.Delay(1000);
        //var status = await powerClient.GetPowerDeviceSwitchAsync(id);
        //device.State = status.state;
        device!.Prosessing = false;
        //var onoff = device.State ? "turned on" : "turned off";
        //if (state != !device.State)
        //{
        //    onoff = "toggled";
        //}
        Snackbar.Add($"{device.Name} toggled!", Severity.Success);
    }
}

public class Device
{
    public int Id { get; init; }
    public string? Mac { get; init; }
    public string? Name { get; init; }
    public bool State { get; set; }
    public bool Prosessing { get; set; }
}

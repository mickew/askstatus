using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using MudBlazor;

namespace Askstatus.Web.App.Pages;

public partial class Home : IAsyncDisposable
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    public NavigationManager? Navigation { get; set; } = null!;

    [Inject]
    private IOptions<AskstatusSettings> Settings { get; set; } = null!;

    public List<Device> Devices { get; set; } = new List<Device>();

    protected bool UserGotNoRights { get; set; } = true;

    private HubConnection? _hubConnection;

    public bool HubIsConnected =>
    _hubConnection?.State == HubConnectionState.Connected;

    public Color HubIsConnectedColor => _hubConnection?.State == HubConnectionState.Connected ? Color.Success : Color.Error;


    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder().WithUrl(Settings.Value.AskStatusSignalRUrl!).Build();

        var response = await ApiService.PowerDeviceAPI.GetPowerDevices();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
        }
        Devices = response.Content!.Select(x => new Device
        {
            Id = x.Id,
            Mac = x.DeviceMac,
            Name = x.Name,
            IsOnline = true,
        }).ToList();
        foreach (var device in Devices)
        {
            var stateResponse = await ApiService.PowerDeviceAPI.GetPowerDeviceStatus(device.Id);
            if (!stateResponse.IsSuccessStatusCode)
            {
                device.IsOnline = false;
                Logger.LogError(stateResponse.Error, stateResponse.Error.Content);
                Snackbar.Add($"{device.Name} is offline", Severity.Warning);
            }
            device.State = stateResponse.Content!;
        }
        _hubConnection.On<int, bool>("UpdateDeviceStatus", (id, onoff) =>
        {
            Logger.LogInformation("UpdateDeviceStatus received");
            var deviceToUpdate = Devices.FirstOrDefault(d => d.Id == id);
            if (deviceToUpdate != null)
            {
                deviceToUpdate.IsOnline = true;
                deviceToUpdate.State = onoff;
                Snackbar.Add($"{deviceToUpdate.Name} turned {BooleanToOnOff(onoff)}!", Severity.Success);
                StateHasChanged();
            }
        });
        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
            Snackbar.Add(ex.Message, Severity.Error);
            return;
        }
        Logger.LogInformation("_hubConnection started");
    }

    private async Task ToggleDevice(int id)
    {
        var device = Devices.Where(x => x.Id == id).FirstOrDefault();
        device!.Prosessing = true;
        StateHasChanged();
        var response = await ApiService.PowerDeviceAPI.TogglePowerDevice(id);
        if (!response.IsSuccessStatusCode)
        {
            device.IsOnline = false;
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            device.Prosessing = false;
            StateHasChanged();
            return;
        }
        await Task.Delay(1000);
        device!.Prosessing = false;
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }

    private string BooleanToOnOff(bool onOff) => onOff ? "on" : "off";
}

public class Device
{
    public int Id { get; init; }
    public string? Mac { get; init; }
    public string? Name { get; init; }
    public bool State { get; set; }
    public bool Prosessing { get; set; }
    public bool IsOnline { get; set; }
}

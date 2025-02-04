﻿using Askstatus.Common.PowerDevice;
using Askstatus.Sdk;
using Askstatus.Web.App.Layout;
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

    public bool HubIsConnected;

    public Color HubIsConnectedColor => _hubConnection?.State == HubConnectionState.Connected ? Color.Success : Color.Error;


    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(Settings.Value.AskStatusSignalRUrl!)
            .WithAutomaticReconnect()
            .Build();

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
            IsOnline = false,
            ChanelType = x.ChanelType,
        }).ToList();

        StateHasChanged();

        _hubConnection.Reconnecting += error =>
        {
            HubIsConnected = false;
            Logger.LogWarning("Trying to reconnect to AskStatus Hub");
            Snackbar.Add("Reconnecting to AskStatus Hub", Severity.Warning);
            StateHasChanged();
            return Task.CompletedTask;
        };

        _hubConnection.Closed += error =>
        {
            HubIsConnected = false;
            Logger.LogWarning("Closed connection to AskStatus Hub");
            //Snackbar.Add("Closed connection to AskStatus SignalR Hub", Severity.Warning);
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            HubIsConnected = true;
            Logger.LogInformation("Reconnected to AskStatus Hub");
            Snackbar.Add("Reconnected to AskStatus SignalR Hub", Severity.Success);
            StateHasChanged();
            return Task.CompletedTask;
        };

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
            HubIsConnected = await ConnectWithRetryAsync(_hubConnection, CancellationToken.None);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex.Message);
            Snackbar.Add(ex.Message, Severity.Error);
            return;
        }
        Logger.LogInformation("_hubConnection started");
        var onlineTasks = new List<Task>();
        foreach (var device in Devices)
        {
            onlineTasks.Add(Task.Run(async () =>
            {
                await GetIsOnline(device);
            }));
        }
        await Task.WhenAll(onlineTasks);
        StateHasChanged();
    }

    public async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
    {
        // Keep trying to until we can start or the token is canceled.
        while (true)
        {
            try
            {

                await connection.StartAsync(token);
                Logger.LogInformation("Connected to AskStatus SignalR Hub");
                return true;
            }
            catch when (token.IsCancellationRequested)
            {
                return false;
            }
            catch
            {
                // Failed to connect, trying again in 5000 ms.
                Logger.LogWarning("Failed to connect to AskStatus SignalR Hub, retrying in 5000 ms");
                Snackbar.Add("Failed to connect to AskStatus SignalR Hub, retrying in 5000 ms", Severity.Warning);
                await Task.Delay(5000);
            }
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

    private static string BooleanToOnOff(bool onOff) => onOff ? "on" : "off";

    private static string ChanelTypeToIcon(ChanelType chanelType, bool state)
    {
        switch
            (chanelType)
        {
            case ChanelType.Generic:
                return state ? AskstatusIcons.GenericOn : AskstatusIcons.GenericOff;
            case ChanelType.Relay:
                return state ? AskstatusIcons.RelayOn : AskstatusIcons.RelayOff;
            case ChanelType.Heat:
                return state ? AskstatusIcons.HeatOn : AskstatusIcons.HeatOff;
            case ChanelType.Bulb:
                return state ? AskstatusIcons.BulbOn : AskstatusIcons.BulbOff;
            default:
                return state ? AskstatusIcons.GenericOn : AskstatusIcons.GenericOff;
        }
    }

    private async Task GetIsOnline(Device device)
    {
        var response = await ApiService.PowerDeviceAPI.GetPowerDeviceStatus(device.Id);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add($"{device.Name} is offline", Severity.Error);
            device.IsOnline = false;
            return;
        }
        device.IsOnline = true;
        device.State = response.Content!;
    }
}

public class Device
{
    public int Id { get; init; }
    public string? Mac { get; init; }
    public string? Name { get; init; }
    public bool State { get; set; }
    public bool Prosessing { get; set; }
    public bool IsOnline { get; set; }
    public ChanelType ChanelType { get; set; }
}

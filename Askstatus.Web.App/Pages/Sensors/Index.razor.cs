using Askstatus.Common.Sensor;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Sensors;

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

    public List<SensorDto> Sensors { get; set; } = new List<SensorDto>();

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiService.SensorAPI.GetSensors();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        Sensors = response.Content!.ToList();
    }
    private async Task EditSensor(SensorDto sensor)
    {
        var parameters = new DialogParameters<EditSensorDialog> { { x => x.sensor, sensor } };

        var dialog = await DialogService.ShowAsync<EditSensorDialog>("Edit Sensor", parameters);
        var result = await dialog.Result;


        if (!result!.Canceled)
        {
            if (result.Data is SensorDto)
            {
                sensor = ((SensorDto)result.Data);
                SensorRequest sensorRequest = new(sensor.Id, sensor.Name, sensor.SensorType, sensor.FormatString, sensor.Name, sensor.SensorModel, sensor.ValueName);
                var res = await ApiService.SensorAPI.UpdateSensor(sensorRequest);
                if (!res.IsSuccessStatusCode)
                {
                    Logger.LogError(res.Error, res.Error.Content);
                    Snackbar.Add(res.Error.Content!, Severity.Error);
                    return;
                }
                Snackbar.Add($"Sensor {sensor.Name} updated", Severity.Success);
            }
        }
    }

    private async Task DeleteSensor(SensorDto sensor)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Warning",
            $"Delete sensor {sensor.Name} {sensor.ValueName} ?",
            yesText: "Delete!", cancelText: "Cancel");
        if (result != null && result.Value)
        {
            var response = await ApiService.SensorAPI.DeleteSensor(sensor.Id);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError(response.Error, response.Error.Content);
                Snackbar.Add(response.Error.Content!, Severity.Error);
                return;
            }
            Sensors.Remove(sensor);
            StateHasChanged();
            Snackbar.Add($"Sensor {sensor.Name} {sensor.ValueName} deleted", Severity.Success);
        }
    }

    private async Task DiscoverSensor()
    {
        var response = await ApiService.SensorDiscoverAPI.NotAssigned();
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            return;
        }
        var flatenedSensors = FlatenSensorInfo(response.Content!);
        await HandleDiscoveredSensorsDialog(flatenedSensors);
    }

    private async Task HandleDiscoveredSensorsDialog(IEnumerable<SensorInfoFlat> sensorInfos)
    {
        var parameters = new DialogParameters<DiscoverAllSensorsDialog> { { x => x.SensorInfos, sensorInfos } };
        var dialog = await DialogService.ShowAsync<DiscoverAllSensorsDialog>("Discover Sensors", parameters);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            if (result.Data is SensorInfoFlat sensorInfo && result.Data is not null)
            {
                var selectedDevice = (SensorInfoFlat)result.Data;

                var sensorRequest = new SensorRequest(0, sensorInfo.Name, SensorType.Unknown, string.Empty, sensorInfo.Id, sensorInfo.SensorModel, sensorInfo.ValueName);
                var res = await ApiService.SensorAPI.CreateSensor(sensorRequest);
                if (!res.IsSuccessStatusCode)
                {
                    Logger.LogError(res.Error, res.Error.Content);
                    Snackbar.Add(res.Error.Content!, Severity.Error);
                    return;
                }
                Sensors.Add(res.Content!);
                Snackbar.Add("Sensor added successfully", Severity.Success);
            }
        }
    }

    private List<SensorInfoFlat> FlatenSensorInfo(IEnumerable<SensorInfo> sensorInfos)
    {
        List<SensorInfoFlat> list = new List<SensorInfoFlat>();
        foreach (var sensor in sensorInfos)
        {
            foreach (var value in sensor.Values)
            {
                list.Add(new SensorInfoFlat(sensor.Id, sensor.Name, sensor.Model, value.Name, value.Value, value.LastUpdate));
            }
        }
        return list;
    }

    public record SensorInfoFlat(string Id, string Name, string SensorModel, string ValueName, string Value, DateTime LastReading);
}

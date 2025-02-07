using Askstatus.Common.PowerDevice;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Devices;

public partial class DiscoverDeviceDialog
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    [Parameter] public PowerDeviceDto Device { get; set; } = new PowerDeviceDto();

    private bool nestedVisible;

    private bool saveDisabled = true;

    private bool Loading = true;

    private bool CloseNestedDisabled;

    private string? Url { get; set; }

    private DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

    protected override void OnInitialized()
    {
        Loading = true;
        saveDisabled = true;
        nestedVisible = true;
        CloseNestedDisabled = true;
    }

    private async Task DiscoverHost(string host)
    {
        try
        {
            Loading = true;
            var response = await ApiService.DeviceDiscoverAPI.Discover(host);
            if (!response.IsSuccessStatusCode)
            {
                Logger.LogError(response.Error, response.Error.Content);
                Snackbar.Add(response.Error.Content!, Severity.Error);
                return;
            }
            Device.Name = response.Content!.DeviceName;
            Device.HostName = response.Content!.DeviceHostName;
            Device.DeviceType = response.Content!.DeviceType;
            Device.DeviceName = response.Content!.DeviceName;
            Device.DeviceId = response.Content!.DeviceId;
            Device.DeviceMac = response.Content!.DeviceMac;
            Device.DeviceModel = response.Content!.DeviceModel;
            Device.Channel = response.Content!.Channel;

            saveDisabled = (string.IsNullOrEmpty(Device.DeviceMac));
            Loading = false;
        }
        catch (Exception)
        {
            Cancel();
        }
    }


    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private void CancelNested()
    {
        nestedVisible = false;
        MudDialog!.CancelAll();
    }

    private void SaveDevice()
    {
        MudDialog!.Close(DialogResult.Ok(Device));
    }

    private async Task CloseNested()
    {
        if (IsValidUri(Url!, out string host))
        {
            nestedVisible = false;
            await DiscoverHost(host);
        }
    }

    void HandleIntervalElapsed(string debouncedText)
    {
        if (IsValidUri(debouncedText, out string host))
        {
            CloseNestedDisabled = false;
        }
        else
        {
            CloseNestedDisabled = true;
        }
    }

    private bool IsValidUri(String uri, out string host)
    {
        try
        {
            Uri u = new Uri(uri);
            Console.WriteLine($"uriBuilder.Host is {u.Host}");
            var b = CheckIPValid(u.Host);
            host = u.Host;
            return b;
        }
        catch
        {
            host = string.Empty;
            return false;
        }
    }

    private bool CheckIPValid(string strIP)
    {
        //  Split string by ".", check that array length is 3
        char chrFullStop = '.';
        string[] arrOctets = strIP.Split(chrFullStop);
        if (arrOctets.Length != 4)
        {
            return false;
        }
        //  Check each substring checking that the int value is less than 255 and that is char[] length is !> 2
        Int16 MAXVALUE = 255;
        Int32 temp; // Parse returns Int32
        foreach (string strOctet in arrOctets)
        {
            if (strOctet.Length > 3)
            {
                return false;
            }

            temp = int.Parse(strOctet);
            if (temp > MAXVALUE)
            {
                return false;
            }
        }
        return true;
    }
}

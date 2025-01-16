using Askstatus.Common.PowerDevice;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Devices;

public partial class EditDeviceDialog
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    [Parameter] public PowerDeviceDto device { get; set; } = new PowerDeviceDto();

    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private void SaveDevice()
    {
        MudDialog!.Close(DialogResult.Ok(device));
    }

}

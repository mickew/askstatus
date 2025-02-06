using Askstatus.Common.PowerDevice;
using Askstatus.Web.App.Layout;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Devices;

public partial class EditDeviceDialog
{
    [CascadingParameter] MudDialogInstance? MudDialog { get; set; }

    [Parameter] public PowerDeviceDto device { get; set; } = new PowerDeviceDto();

    private DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

    private void Cancel()
    {
        MudDialog!.Cancel();
    }

    private void SaveDevice()
    {
        MudDialog!.Close(DialogResult.Ok(device));
    }

    private string ChanelTypeToIcon(ChanelType chanelType)
    {
        switch
            (chanelType)
        {
            case ChanelType.Generic:
                return AskstatusIcons.GenericOn;
            case ChanelType.Relay:
                return AskstatusIcons.RelayOn;
            case ChanelType.Heat:
                return AskstatusIcons.HeatOn;
            case ChanelType.Bulb:
                return AskstatusIcons.BulbOn;
            default:
                return AskstatusIcons.GenericOn;
        }
    }
}

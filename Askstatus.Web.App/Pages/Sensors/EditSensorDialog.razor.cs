using Askstatus.Common.Sensor;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Sensors;

public partial class EditSensorDialog
{
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    [Parameter] public SensorDto sensor { get; set; } = new SensorDto();

    private DefaultFocus DefaultFocus { get; set; } = DefaultFocus.FirstChild;

    private void Cancel()
    {
        MudDialog!.Cancel();
    }
    private void SaveSensor()
    {
        MudDialog!.Close(DialogResult.Ok(sensor));
    }

}

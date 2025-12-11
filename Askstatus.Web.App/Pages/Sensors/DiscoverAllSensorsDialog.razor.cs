using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static Askstatus.Web.App.Pages.Sensors.Index;

namespace Askstatus.Web.App.Pages.Sensors;

public partial class DiscoverAllSensorsDialog
{
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    private bool _addDisabled = true;

    private SensorInfoFlat? _selectedItem;

    [Parameter] public IEnumerable<SensorInfoFlat> SensorInfos { get; set; } = Array.Empty<SensorInfoFlat>();

    private void Submit() => MudDialog!.Close(DialogResult.Ok(_selectedItem));

    private void Cancel() => MudDialog!.Cancel();

    private void OnSelectedItemChanged(SensorInfoFlat item)
    {
        _selectedItem = item;
        _addDisabled = item == null;
    }

    private string UtcToLocalTimeFormat(DateTime dateTime)
    {
        return dateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }
}

using Askstatus.Common.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.Devices;

public partial class DiscoverAllDevicesDialog
{
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }

    //private int _selectedRowNumber = -1;

    //private MudTable<DicoverInfo>? _mudTable;
    private bool _addDisabled = true;

    private DicoverInfo? _selectedItem;

    [Parameter] public IEnumerable<DicoverInfo> DicoverInfos { get; set; } = Array.Empty<DicoverInfo>();

    private void Submit() => MudDialog!.Close(DialogResult.Ok(_selectedItem));

    private void Cancel() => MudDialog!.Cancel();

    private void OnSelectedItemChanged(DicoverInfo item)
    {
        _selectedItem = item;
        _addDisabled = item == null;
    }
    //private string SelectedRowClassFunc(DicoverInfo dicoverInfo, int rowNumber)
    //{
    //    if (_selectedRowNumber == rowNumber)
    //    {
    //        _selectedRowNumber = -1;
    //        return "string.Empty";
    //    }
    //    else if (_mudTable!.SelectedItem != null && _mudTable.SelectedItem.Equals(dicoverInfo))
    //    {
    //        _selectedRowNumber = rowNumber;
    //        return "selected";
    //    }
    //    else
    //    {
    //        return string.Empty;
    //    }
    //}
    //private void RowClickEvent(TableRowClickEventArgs<DicoverInfo> tableRowClickEventArgs)
    //{
    //}
}

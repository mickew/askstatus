using Askstatus.Common.System;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Pages.AskStatus;

public partial class EventLog
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private ILogger<Index> Logger { get; set; } = null!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = null!;

    private MudDataGrid<SystemLogDto>? _dataGrid;
    private string _searchString = string.Empty;

    private readonly int[] _pageSizeOptions = { 10, 25, 50 };
    private async Task<GridData<SystemLogDto>> ServerReload(GridState<SystemLogDto> state)
    {
        var desc = true;
        string sortColumn = string.Empty;
        var sortDefinition = state.SortDefinitions.FirstOrDefault();
        if (sortDefinition != null)
        {
            sortColumn = sortDefinition.SortBy;
            desc = sortDefinition.Descending;
        }

        var response = await ApiService.SystemAPI.GetSystemInfo(_searchString, sortColumn, state.Page + 1, state.PageSize, desc);
        if (!response.IsSuccessStatusCode)
        {
            Logger.LogError(response.Error, response.Error.Content);
            Snackbar.Add(response.Error.Content!, Severity.Error);
            StateHasChanged();
            return new GridData<SystemLogDto>
            {
                TotalItems = 0,
                Items = Array.Empty<SystemLogDto>()
            };
        }
        return new GridData<SystemLogDto>
        {

            TotalItems = response.Content!.TotalCount,
            Items = response.Content!.Data
        };
    }

    private Task OnSearch(string text)
    {
        _searchString = text;
        return _dataGrid!.ReloadServerData();
    }

}

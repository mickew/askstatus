using Askstatus.Common.System;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;

namespace Askstatus.Web.App.Pages.AskStatus;

public partial class SystemInfo
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    private SystemInfoDto? _systemInfo;

    protected override async Task OnInitializedAsync()
    {
        var response = await ApiService.SystemAPI.GetSystemInfo();
        if (response.IsSuccessStatusCode)
        {
            _systemInfo = response.Content;
        }
    }
}

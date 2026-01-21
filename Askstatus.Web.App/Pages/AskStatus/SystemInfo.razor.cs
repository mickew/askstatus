using Askstatus.Common.System;
using Askstatus.Sdk;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Askstatus.Web.App.Pages.AskStatus;

public partial class SystemInfo
{
    [Inject]
    private AskstatusApiService ApiService { get; set; } = null!;

    [Inject]
    private IWebAssemblyHostEnvironment HostEnvironment { get; set; } = null!;

    private SystemInfoDto? _systemInfo;

    private string? _environmentName;

    protected override async Task OnInitializedAsync()
    {
        _environmentName = HostEnvironment.Environment;

        var response = await ApiService.SystemAPI.GetSystemInfo();
        if (response.IsSuccessStatusCode)
        {
            _systemInfo = response.Content;
        }
    }
}

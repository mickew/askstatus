using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Layout;

public partial class MainLayout : LayoutComponentBase
{
    private bool _isDarkMode;

    private bool _drawerOpen = false;

    private MudThemeProvider? _mudThemeProvider;

    protected string Version { get; private set; } = string.Empty;

    protected string AspDotnetVersion { get; private set; } = string.Empty;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private void DarkModeToggle()
    {
        _isDarkMode = !_isDarkMode;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _isDarkMode = await _mudThemeProvider!.GetSystemPreference();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        Assembly currentAssembly = typeof(MainLayout).Assembly;
        if (currentAssembly == null)
        {
            currentAssembly = Assembly.GetCallingAssembly();
        }

        AspDotnetVersion = currentAssembly.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName!;
        Version = $"{currentAssembly.GetName().Version!.Major}.{currentAssembly.GetName().Version!.Minor}.{currentAssembly.GetName().Version!.Build}";
        await base.OnInitializedAsync();
    }
}

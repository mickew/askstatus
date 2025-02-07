using MudBlazor;

namespace Askstatus.Web.App.Layout;

public partial class MainLayout : LayoutBase
{
    private bool _isDarkMode;

    private bool _drawerOpen = false;

    private MudThemeProvider? _mudThemeProvider;

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
}

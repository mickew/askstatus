using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Askstatus.Web.App.Layout;

public partial class MainLayout : LayoutBase
{
    private const string dataKey = "modeKey";

    [Inject]
    Blazored.LocalStorage.ILocalStorageService _localStorageService { get; set; } = null!;

    private bool _isDarkMode;

    private int _index = 0;

    private readonly string[] _modeIcons = { Icons.Material.Filled.WbSunny, Icons.Material.Filled.ModeNight, Icons.Custom.Brands.Chrome };

    private bool _drawerOpen = false;

    private MudThemeProvider? _mudThemeProvider;

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task DarkModeToggle()
    {
        _index = (_index + 1) % 3;
        _isDarkMode = _index switch
        {
            0 => false,
            1 => true,
            2 => await _mudThemeProvider!.GetSystemPreference(),
            _ => false
        };
        await StoreLocalData();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RetrieveLocalData();
            _isDarkMode = _index switch
            {
                0 => false,
                1 => true,
                2 => await _mudThemeProvider!.GetSystemPreference(),
                _ => false
            };
        }
    }

    private async Task StoreLocalData()
    {
        await _localStorageService.SetItemAsync(dataKey, _index);
    }

    private async Task RetrieveLocalData()
    {
        if (await _localStorageService.ContainKeyAsync(dataKey))
        {
            _index = await _localStorageService.GetItemAsync<int>(dataKey);
        }
    }

}

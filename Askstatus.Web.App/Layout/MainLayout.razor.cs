using Microsoft.AspNetCore.Components;
using MudBlazor;
using Toolbelt.Blazor.HotKeys2;

namespace Askstatus.Web.App.Layout;

public partial class MainLayout : LayoutBase, IAsyncDisposable
{
    private const string dataKey = "modeKey";

    [Inject]
    Blazored.LocalStorage.ILocalStorageService _localStorageService { get; set; } = null!;

    [Inject]
    HotKeys _hotKeys { get; set; } = null!;

    [Inject]
    NavigationManager _navigationManager { get; set; } = null!;

    private bool _isDarkMode;

    private int _index = 0;

    private readonly string[] _modeIcons = { Icons.Material.Filled.WbSunny, Icons.Material.Filled.ModeNight, Icons.Custom.Brands.Chrome };

    private bool _drawerOpen = false;

    private HotKeysContext? _hotKeysContext;

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
            _hotKeysContext = _hotKeys.CreateContext()
                .Add(ModCode.Ctrl, Code.H, () => GoTo("/"), "Go to Home page.")
                .Add(ModCode.Ctrl, Code.U, () => GoTo("/admin/users"), "Go to Users.")
                .Add(ModCode.Ctrl, Code.A, () => GoTo("/admin/access-control"), "Go to Access control.")
                .Add(ModCode.Ctrl, Code.R, () => GoTo("/admin/roles"), "Go to Roles.")
                .Add(ModCode.Ctrl, Code.D, () => GoTo("/admin/devices"), "Go to Devics.")
                .Add(ModCode.Ctrl, Code.Comma, () => DrawerToggle());
        }
    }

    private ValueTask GoTo(string url)
    {
        var urlToNavigate = _navigationManager.BaseUri.TrimEnd('/') + "/" + url.TrimStart('/');
        _navigationManager.NavigateTo(urlToNavigate);
        return ValueTask.CompletedTask;
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

    public async ValueTask DisposeAsync()
    {
        if (_hotKeysContext != null)
        {
            await _hotKeysContext.DisposeAsync();
        }
    }
}
